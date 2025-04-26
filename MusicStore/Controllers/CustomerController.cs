using MusicStore.Model.Abstract;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Models.Admin.Customer;
using MusicStore.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MusicStore.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IEntitiesRepository<Order> _orderRepository;
        private readonly IEntitiesRepository<Customer> _customerRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(
            IEntitiesRepository<Order> orderRepository,
            IEntitiesRepository<Customer> customerRepository,
            UserManager<ApplicationUser> userManager)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string FirstNameFilter, string LastNameFilter)
        {
            // First get the base query from the repository
            var customersQuery = _customerRepository.GetAll();

            // Materialize the data first, then apply filters in memory
            var customers = customersQuery.ToList();

            // Now filter in memory with your original logic
            if (!string.IsNullOrEmpty(FirstNameFilter))
            {
                customers = customers.Where(c => c.firstName != null &&
                    c.firstName.Contains(FirstNameFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(LastNameFilter))
            {
                customers = customers.Where(c => c.lastName != null &&
                    c.lastName.Contains(LastNameFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            
            var customerViewModels = new List<CustomerViewModel>();
            foreach (var customer in customers)
            {
                var customerViewModel = new CustomerViewModel
                {
                    CustomerId = customer.Id,
                    UserId = customer.UserId,
                    FirstName = customer.firstName,
                    LastName = customer.lastName,
                    Phone = customer.Phone,
                    Address = customer.Address,
                    CreatedAt = customer.createdAt,
                    UpdatedAt = customer.updatedAt
                };

                var user = await _userManager.FindByIdAsync(customer.UserId.ToString());
                if (user != null)
                {
                    customerViewModel.Email = user.Email;
                    customerViewModel.IsActive = user.IsActive;
                }

                customerViewModels.Add(customerViewModel);
            }

            var listViewModel = new CustomerListViewModel
            {
                FirstNameFilter = FirstNameFilter,
                LastNameFilter = LastNameFilter,
                Customers = customerViewModels
            };

            return View(listViewModel);
        }




        [HttpGet]
        public async Task<ActionResult> Create()
        {
            // Get users who don't already have a customer profile
            var existingCustomerUserIds = _customerRepository.GetAll().Select(c => c.UserId).ToList();
            var availableUsers = await _userManager.Users
                .Where(u => !existingCustomerUserIds.Contains(u.Id))
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.LastName}, {u.FirstName} ({u.Email})"
                })
                .ToListAsync();

            ViewBag.AvailableUsers = availableUsers;

            return View(new CustomerViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var existingCustomerUserIds = _customerRepository.GetAll().Select(c => c.UserId).ToList();
                var availableUsers = await _userManager.Users
                    .Where(u => !existingCustomerUserIds.Contains(u.Id))
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = $"{u.LastName}, {u.FirstName} ({u.Email})"
                    })
                    .ToListAsync();

                ViewBag.AvailableUsers = availableUsers;
                return View(model);
            }

            // Check if this user already has a customer profile
            var existingCustomer = _customerRepository.GetAll()
                .FirstOrDefault(c => c.UserId == model.UserId);

            if (existingCustomer != null)
            {
                ModelState.AddModelError("UserId", "This user already has a customer profile");
                return View(model);
            }

            // Get user info to sync with customer
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                ModelState.AddModelError("UserId", "User not found");
                return View(model);
            }

            var customer = new Customer
            {
                UserId = model.UserId,
                firstName = model.FirstName ?? user.FirstName, // Use model data or fallback to user data
                lastName = model.LastName ?? user.LastName,
                Address = model.Address,
                Phone = model.Phone ?? user.PhoneNumber,
                createdAt = DateTime.Now,
                updatedAt = DateTime.Now
            };

            _customerRepository.Add(customer);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var customer = _customerRepository.Get(id);
            if (customer == null)
            {
                return NotFound();
            }

            var model = new CustomerViewModel
            {
                CustomerId = customer.Id,
                UserId = customer.UserId,
                FirstName = customer.firstName,
                LastName = customer.lastName,
                Address = customer.Address,
                Phone = customer.Phone,
                CreatedAt = customer.createdAt,
                UpdatedAt = customer.updatedAt
            };

            // Get user information
            var user = await _userManager.FindByIdAsync(customer.UserId.ToString());
            if (user != null)
            {
                model.Email = user.Email;
                model.IsActive = user.IsActive;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = _customerRepository.Get(model.CustomerId);
            if (customer == null)
            {
                return NotFound();
            }

            // Update customer properties
            customer.firstName = model.FirstName;
            customer.lastName = model.LastName;
            customer.Address = model.Address;
            customer.Phone = model.Phone;
            customer.updatedAt = DateTime.Now;

            _customerRepository.Update(customer);

            // Optionally sync some data with user
            var user = await _userManager.FindByIdAsync(customer.UserId.ToString());
            if (user != null)
            {
                bool userChanged = false;

                // Only update user if data is different
                if (user.FirstName != model.FirstName)
                {
                    user.FirstName = model.FirstName;
                    userChanged = true;
                }

                if (user.LastName != model.LastName)
                {
                    user.LastName = model.LastName;
                    userChanged = true;
                }

                if (user.PhoneNumber != model.Phone)
                {
                    user.PhoneNumber = model.Phone;
                    userChanged = true;
                }

                if (userChanged)
                {
                    user.UpdatedOnUtc = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            // Get the customer with the specified ID
            var customer = _customerRepository.Get(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Get all orders for this customer
            var customerOrders = _orderRepository.GetAll()
                .Where(o => o.CustomerId == id)
                .ToList();

            // Get associated user information
            var user = await _userManager.FindByIdAsync(customer.UserId.ToString());

            var model = new CustomerViewModel
            {
                CustomerId = customer.Id,
                UserId = customer.UserId,
                FirstName = customer.firstName,
                LastName = customer.lastName,
                Address = customer.Address,
                Phone = customer.Phone,
                CreatedAt = customer.createdAt,
                UpdatedAt = customer.updatedAt,
                Orders = customerOrders
            };

            if (user != null)
            {
                model.Email = user.Email;
                model.IsActive = user.IsActive;
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var customer = _customerRepository.Get(id);
            if (customer == null)
            {
                return NotFound();
            }

            var customerOrders = _orderRepository.GetAll()
                .Where(o => o.CustomerId == id)
                .ToList();

            var user = await _userManager.FindByIdAsync(customer.UserId.ToString());

            var model = new CustomerViewModel
            {
                CustomerId = customer.Id,
                UserId = customer.UserId,
                FirstName = customer.firstName,
                LastName = customer.lastName,
                Address = customer.Address,
                Phone = customer.Phone,
                CreatedAt = customer.createdAt,
                UpdatedAt = customer.updatedAt,
                Orders = customerOrders
            };

            if (user != null)
            {
                model.Email = user.Email;
                model.IsActive = user.IsActive;
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var customer = _customerRepository.Get(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Check if customer has orders
            var hasOrders = _orderRepository.GetAll().Any(o => o.CustomerId == id);
            if (hasOrders)
            {
                ModelState.AddModelError("", "Cannot delete customer with existing orders");
                return RedirectToAction("Delete", new { id = id });
            }

            _customerRepository.Remove(customer);
            return RedirectToAction("Index");
        }
    }
}