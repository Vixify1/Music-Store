using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Model.Abstract;
using MusicStore.Model.Entities;
using MusicStore.Models.UserProfile;
using System.Threading.Tasks;

namespace MusicStore.Controllers
{
    [Authorize] // Ensure user is logged in
    public class UserProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEntitiesRepository<Customer> _customerRepository;

        public UserProfileController(
            UserManager<ApplicationUser> userManager,
            IEntitiesRepository<Customer> customerRepository)
        {
            _userManager = userManager;
            _customerRepository = customerRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Check if user is admin
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            // Check if there's a customer profile
            var customer = _customerRepository.GetAll()
                .FirstOrDefault(c => c.UserId == user.Id);

            var model = new UserProfileViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                HasCustomerProfile = customer != null,
                IsAdmin = isAdmin
            };

            if (customer != null)
            {
                model.CustomerId = customer.Id;
                model.Address = customer.Address;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Update user
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.UpdatedOnUtc = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("Index", model);
            }

            // Find or create customer profile
            var customer = _customerRepository.GetAll()
                .FirstOrDefault(c => c.UserId == user.Id);

            if (customer == null)
            {
                // Create new customer profile
                customer = new Customer
                {
                    UserId = user.Id,
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    Phone = model.PhoneNumber,
                    Address = model.Address,
                    createdAt = DateTime.Now,
                    updatedAt = DateTime.Now
                };
                _customerRepository.Add(customer);
            }
            else
            {
                // Update existing customer
                customer.firstName = model.FirstName;
                customer.lastName = model.LastName;
                customer.Phone = model.PhoneNumber;
                customer.Address = model.Address;
                customer.updatedAt = DateTime.Now;
                _customerRepository.Update(customer);
            }

            TempData["StatusMessage"] = "Your profile has been updated";
            return RedirectToAction(nameof(Index));
        }
    }
}