using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicStore.Model.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using MusicStore.Model.Context;
using MusicStore.Model.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace MusicStore.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IEntitiesRepository<Reviews> _reviewRepository;
        private readonly IEntitiesRepository<Album> _albumRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEntitiesRepository<Customer> _customerRepository;
        private readonly IEntitiesRepository<Order> _orderRepository;


        public ReviewController(IEntitiesRepository<Reviews> reviewRepository, IEntitiesRepository<Album> albumRepository, UserManager<ApplicationUser> userManager, IEntitiesRepository<Customer> customerRepository, IEntitiesRepository<Order> orderRepository)
        {
            _reviewRepository = reviewRepository;
            _albumRepository = albumRepository;
            _userManager = userManager;
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
        }


        // Helper method to get current customer ID
        private async Task<int?> GetCurrentCustomerIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;

            var customer = await _customerRepository
                .GetAsync(c => c.UserId == user.Id);

            return customer?.Id;
        }


        // GET: Review
        public async Task<IActionResult> Index()
        {
            //var customerId = await GetCurrentCustomerIdAsync();

            var reviews = await _reviewRepository.GetAll().
                Include(r => r.Customer)
                .ThenInclude(r => r.User)
                .Include(r => r.Album)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reviews);
        }

        // GET: Review/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var review = await _reviewRepository.GetAll()
               .Include(r => r.Customer)
               .ThenInclude(c => c.User)
               .Include(r => r.Album)
               .FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null)
                return NotFound();

            return View(review);
        }


        // Then later we can add the admin for post/delete/put actions
       
        // GET: Review/Create
        [Authorize] // Ensure user is logged in
        public async Task<IActionResult> Create()
        {
            var customerId = await GetCurrentCustomerIdAsync();
            if (customerId == null)
                return Forbid(); // User is not a customer

            // Get albums the customer has ordered
            var orderedAlbumIds = await _orderRepository.GetAll()
                .Where(o => o.CustomerId == customerId)
                .SelectMany(o => o.OrderItems)
                .Select(od => od.AlbumId)
                .Distinct()
                .ToListAsync();

            if (orderedAlbumIds.Count == 0)
            {
                // No albums ordered
                ViewBag.NoOrderedAlbums = true;
                return View();
            }

            // Get ordered albums that haven't been reviewed yet
            var existingReviewedAlbumIds = await _reviewRepository.GetAll()
                .Where(r => r.CustomerId == customerId)
                .Select(r => r.AlbumId)
                .ToListAsync();

            var availableAlbumIds = orderedAlbumIds
                .Except(existingReviewedAlbumIds)
                .ToList();

            var availableAlbums = await _albumRepository.GetAll()
                .Where(a => availableAlbumIds.Contains(a.Id))
                .ToListAsync();

            ViewBag.CustomerId = customerId;
            ViewBag.Albums = new SelectList(availableAlbums, "Id", "Title");

            return View();
        }

        // POST: Review/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Ensure user is logged in
        public async Task<IActionResult> Create(Reviews review)
        {
            var customerId = await GetCurrentCustomerIdAsync();
            if (customerId == null || customerId != review.CustomerId)
                return Forbid(); // Not the customer or trying to submit as someone else

            // Verify this customer has ordered this album
            var hasOrdered = await _orderRepository.GetAll()
                .Where(o => o.CustomerId == customerId)
                .SelectMany(o => o.OrderItems)
                .AnyAsync(od => od.AlbumId == review.AlbumId);

            if (!hasOrdered)
                return Forbid(); // Customer hasn't ordered this album

            // Check if customer already reviewed this album
            var existingReview = await _reviewRepository.GetAsync(
                r => r.CustomerId == customerId && r.AlbumId == review.AlbumId);

            if (existingReview != null)
            {
                ModelState.AddModelError("", "You have already reviewed this album.");

                // Re-populate dropdown for ordered albums
                var customerOrderedAlbumIds = await _orderRepository.GetAll()
                    .Where(o => o.CustomerId == customerId)
                    .SelectMany(o => o.OrderItems)
                    .Select(od => od.AlbumId)
                    .Distinct()
                    .ToListAsync();

                var availableAlbums = await _albumRepository.GetAll()
                    .Where(a => customerOrderedAlbumIds.Contains(a.Id))
                    .ToListAsync();

                ViewBag.CustomerId = customerId;
                ViewBag.Albums = new SelectList(availableAlbums, "Id", "Title", review.AlbumId);

                return View(review);
            }

            if (review.AlbumId > 0 && review.CustomerId > 0 &&
                !string.IsNullOrEmpty(review.Comment) &&
                review.Rating >= 1 && review.Rating <= 5)
            {
                review.CreatedAt = DateTime.Now;
                _reviewRepository.Add(review);
                return RedirectToAction(nameof(Index));
            }

            // If we get here, there was an issue with validation
            // Re-populate the dropdown
            ViewBag.CustomerId = customerId;

            // First get the album IDs from customer's orders
            var orderedAlbumIds = await _orderRepository.GetAll()
                .Where(o => o.CustomerId == customerId)
                .SelectMany(o => o.OrderItems)
                .Select(od => od.AlbumId)
                .Distinct()
                .ToListAsync();

            // Then get the albums that match those IDs
            var albums = await _albumRepository.GetAll()
                .Where(a => orderedAlbumIds.Contains(a.Id))
                .ToListAsync();

            ViewBag.Albums = new SelectList(albums, "Id", "Title", review.AlbumId);

            return View(review);
        }
        // GET: Review/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var review = await _reviewRepository.GetAsync(id);
            if (review == null)
                return NotFound();

            // Use _albumRepository to populate the dropdown
            var albums = await _albumRepository.GetAllAsync();
            ViewBag.Albums = new SelectList(albums, "Id", "Title", review.AlbumId);

            return View(review);

         
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reviews review)
        {
            if (id != review.ReviewId)
                return NotFound();

            //// If ModelState is invalid, check what's causing it
            if (!ModelState.IsValid)
            {
                // For debugging - This will help you see the validation errors
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
            }

            // Continue with the update even if there are some model validation issues , just annoying for debugging
            try
            {
                // Get the original review to preserve existing data
                var originalReview = await _reviewRepository
                    .GetAsync(r => r.ReviewId == id);

                if (originalReview != null)
                {
                    // Keep original values for fields not included in the form
                    review.CreatedAt = originalReview.CreatedAt;

                    // If CustomerId is not being sent in the form, maintain it
                    if (review.CustomerId <= 0)
                    {
                        review.CustomerId = originalReview.CustomerId;
                    }

                    // Update only specific fields instead of the entire entity
                    _reviewRepository.Update(originalReview, review); // Safely copies only the changed values                    
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DbUpdateConcurrencyException)
            {

                var existingReview = await _reviewRepository.GetAsync(id);
                if ( existingReview== null)
                    return NotFound();
                else
                    throw;
            }
            catch (Exception ex)
            {
                // Add the error to ModelState
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
            }

            // If we get here, something went wrong, repopulate the dropdown
            ViewBag.Products = new SelectList(await _reviewRepository.GetAllAsync(), "AlbumId", "Title", review.AlbumId);
            return View(review);
        }

        // GET: Review/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var review = await _reviewRepository.GetAll()
                .Include(r => r.Customer).
                ThenInclude(c => c.User)
                .Include(r => r.Album)
                .FirstOrDefaultAsync(m => m.ReviewId == id);

            if (review == null)
                return NotFound();

            return View(review);
        }

        // POST: Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _reviewRepository.GetAsync(id);

            if (review != null)
            {
                _reviewRepository.Remove(review);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}