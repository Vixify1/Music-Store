using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Controllers;
using MusicStore.Model.Abstract;
using MusicStore.Model.Entities;
using MusicStore.Models.Wishlist;

public class WishlistController : Controller
{
    private readonly IEntitiesRepository<Wishlist> _wishlistRepository;
    private readonly IEntitiesRepository<Customer> _customerRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEntitiesRepository<Order> _orderRepository;
    private readonly IEntitiesRepository<Album> _albumRepository;
    private readonly IEntitiesRepository<Cart> _cartRepository;

    public WishlistController(

        IEntitiesRepository<Wishlist> wishlistRepository,
        IEntitiesRepository<Customer> customerRepository,
        UserManager<ApplicationUser> userManager,
        IEntitiesRepository<Order> orderRepository,
        IEntitiesRepository<Album> albumRepository,
        IEntitiesRepository<Cart> cartRepository)
      
        
    {
        _wishlistRepository = wishlistRepository;
        _customerRepository = customerRepository;
        _userManager = userManager;
        _orderRepository = orderRepository;
        _albumRepository = albumRepository;
        _cartRepository = cartRepository;
    }

    // Helper method to get current customer ID
    private async Task<int> GetCurrentCustomerIdAsync()
    {
        // Get the current logged-in user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        // Find the customer record associated with this user
        var customer = _customerRepository.Get(c => c.UserId == user.Id);
        if (customer == null)
        {
            throw new UnauthorizedAccessException("Customer profile not found for this user");
        }

        return customer.Id;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        try
        {
            int customerId = await GetCurrentCustomerIdAsync();

            // Get wishlist items with their associated Albums
            var wishlistItems = _wishlistRepository.GetSome<Album>(
                w => w.CustomerId == customerId,
                w => w.Album).ToList();

            return View(wishlistItems);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Authentication", new { returnUrl = Request.Path });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddToWishlist(int albumId)
    {
        try
        {
            int customerId = await GetCurrentCustomerIdAsync();


            ////// TESTING ONLY: Force Id to 1
            //Id = 1;

            // Check if Album already exists in wishlist
            var existingItem = _wishlistRepository.Get(
                w => w.CustomerId == customerId && w.AlbumId == albumId);

            if (existingItem != null)
            {
                return Json(new { success = false, message = "Album already in wishlist" });
            }

            // Create new wishlist item
            var wishlistItem = new Wishlist
            {
                AlbumId = albumId,
                CustomerId = customerId,
                CreatedAt = DateTime.Now
            };

            _wishlistRepository.Add(wishlistItem);
            _wishlistRepository.SaveChanges();


            return RedirectToAction("Index");
            //return Json(new { success = true, message = "Album added to wishlist" });
        }
        catch (UnauthorizedAccessException)
        {
            return Json(new { success = false, message = "User not authenticated or customer profile not found" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "An error occurred while adding Album to wishlist" });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RemoveFromWishlist(int wishlistId)
    {
        try
        {
            int customerId = await GetCurrentCustomerIdAsync();

            // Find the wishlist item and verify ownership
            var wishlistItem = _wishlistRepository.Get(
                w => w.WishlistId == wishlistId && w.CustomerId == customerId);

            if (wishlistItem == null)
            {
                // Item not found or doesn't belong to this customer
                return NotFound();
            }

            _wishlistRepository.Remove(wishlistItem);
            _wishlistRepository.SaveChanges();

            return RedirectToAction("Index");
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Authentication", new { returnUrl = Request.Path });
        }
        catch (Exception)
        {
            // Handle other exceptions
            // You might want to add a temp data message here
            return RedirectToAction("Index");
        }
    }


    //[HttpGet]
    //public IActionResult Checkout()
    //{
    //    var user = _userManager.GetUserAsync(User).Result;
    //    var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);

    //    if (customer == null)
    //    {
    //        return RedirectToAction("Login", "Account");
    //    }

    //    var wishlistEntries = _wishlistRepository.GetAll()
    //        .Where(w => w.CustomerId == customer.Id)
    //        .Include(w => w.Album)
    //        .ToList();

    //    if (!wishlistEntries.Any())
    //    {
    //        TempData["Message"] = "Your wishlist is empty.";
    //        return RedirectToAction("Index");
    //    }

    //    var viewModel = new WishlistCheckoutViewModel
    //    {
    //        Items = wishlistEntries.Select(w => new WishlistItemViewModel
    //        {
    //            WishlistId = w.WishlistId,
    //            AlbumId = w.AlbumId,
    //            Title = w.Album.Title,
    //            Artist = w.Album.Artist,
    //            Price = w.Album.Price
    //        }).ToList(),
    //        TotalAmount = wishlistEntries.Sum(w => w.Album.Price)
    //    };

    //    return View(viewModel);
    //}


    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public IActionResult Checkout(WishlistCheckoutViewModel model)
    //{
    //    var user = _userManager.GetUserAsync(User).Result;
    //    var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);

    //    if (customer == null)
    //    {
    //        return RedirectToAction("Login", "Account");
    //    }

    //    var wishlistEntries = _wishlistRepository.GetAll()
    //        .Where(w => w.CustomerId == customer.Id)
    //        .Include(w => w.Album)
    //        .ToList();

    //    if (!wishlistEntries.Any())
    //    {
    //        TempData["Message"] = "Your wishlist is empty.";
    //        return RedirectToAction("Index");
    //    }

    //    // Create new Order
    //    var order = new Order
    //    {
    //        CustomerId = customer.Id,
    //        OrderDate = DateTime.Now,
    //        TotalAmount = model.TotalAmount,
    //        //ShippingAddress = model.ShippingAddress,
    //        //PaymentMethod = model.PaymentMethod,
    //        OrderItems = wishlistEntries.Select(w => new OrderItems
    //        {
    //            Id = w.AlbumId,
    //            Quantity = 1,
    //            UnitPrice = w.Album.Price
    //        }).ToList()
    //    };

    //    _orderRepository.Add(order);

    //    TempData["SuccessMessage"] = "Order placed successfully from wishlist!";
    //    return RedirectToAction("Index", "Home");
    //}



    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddToCart(int wishlistId)
    {
        try
        {
            int customerId = await GetCurrentCustomerIdAsync();

            // Find the wishlist item
            var wishlistItem = _wishlistRepository.Get(
                w => w.WishlistId == wishlistId && w.CustomerId == customerId,
                w => w.Album);

            if (wishlistItem == null)
            {
                TempData["ErrorMessage"] = "Wishlist item not found.";
                return RedirectToAction("Index");
            }

            // Get the album from the wishlist item
            var albumId = wishlistItem.AlbumId;

            // Use the existing cart controller to add the item
            var cartController = new CartController(
                _albumRepository,
                _userManager,
                _customerRepository,
                _cartRepository);

            // Set the controller context
            cartController.ControllerContext = this.ControllerContext;

            // Add the album to the cart
            cartController.AddToCart(albumId);

            // Optional Remove from wishlist after adding to cart
            // _wishlistRepository.Remove(wishlistItem);
            // _wishlistRepository.SaveChanges();

            TempData["SuccessMessage"] = "Album added to cart.";
            return RedirectToAction("Index");
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Authentication", new { returnUrl = Request.Path });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred while adding to cart.";
            return RedirectToAction("Index");
        }


    }
    [HttpPost]
    [Authorize]
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddAllToCart()
    {
        try
        {
            int customerId = await GetCurrentCustomerIdAsync();

            // Get all wishlist items for the current customer
            // Fix: Separate the filter predicate from the include operation
            var wishlistItems = _wishlistRepository.GetAll()
                .Where(w => w.CustomerId == customerId)
                .Include(w => w.Album)
                .ToList();

            if (wishlistItems == null || !wishlistItems.Any())
            {
                TempData["ErrorMessage"] = "No items in wishlist.";
                return RedirectToAction("Index");
            }

            // Use the existing cart controller to add each item
            var cartController = new CartController(
                _albumRepository,
                _userManager,
                _customerRepository,
                _cartRepository);

            // Set the controller context
            cartController.ControllerContext = this.ControllerContext;

            int successCount = 0;

            // Add each album to the cart
            foreach (var wishlistItem in wishlistItems)
            {
                try
                {
                    var albumId = wishlistItem.AlbumId;
                    cartController.AddToCart(albumId);
                    successCount++;

                    // Optional: Remove from wishlist after adding to cart
                    // _wishlistRepository.Remove(wishlistItem);
                }
                catch (Exception ex)
                {
                    // Log the error but continue with the next item
                    // _logger.LogError($"Failed to add album ID {wishlistItem.AlbumId} to cart: {ex.Message}");
                }
            }

            // Save changes after processing all items
            // if (removeFromWishlist)
            // {
            //     _wishlistRepository.SaveChanges();
            // }

            if (successCount == 0)
            {
                TempData["ErrorMessage"] = "Failed to add any albums to cart.";
            }
            else if (successCount < wishlistItems.Count())
            {
                TempData["WarningMessage"] = $"Added {successCount} out of {wishlistItems.Count()} albums to cart.";
            }
            else
            {
                TempData["SuccessMessage"] = "All albums added to cart successfully.";
            }

            return RedirectToAction("Index");
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Authentication", new { returnUrl = Request.Path });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred while adding items to cart.";
            return RedirectToAction("Index");
        }
    }

}