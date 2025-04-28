using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Helper;
using MusicStore.Model.Abstract;
using MusicStore.Model.Entities;
using MusicStore.Models;
using Microsoft.EntityFrameworkCore;

namespace MusicStore.Controllers
{
    public class CartController : Controller
    {
        private readonly IEntitiesRepository<Album> _albumRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEntitiesRepository<Customer> _customerRepository;
        private readonly IEntitiesRepository<Cart> _cartRepository;

        public CartController(
            IEntitiesRepository<Album> albumRepository,
            UserManager<ApplicationUser> userManager,
            IEntitiesRepository<Customer> customerRepository,
            IEntitiesRepository<Cart> cartRepository)
        {
            _albumRepository = albumRepository;
            _userManager = userManager;
            _customerRepository = customerRepository;
            _cartRepository = cartRepository;
        }

        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);

            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get cart with included items and albums
            var cart = _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Album)
                .FirstOrDefault(c => c.CustomerId == customer.Id);

            if (cart == null)
            {
                cart = new Cart { CustomerId = customer.Id };
                _cartRepository.Add(cart);
                _cartRepository.SaveChanges();
            }

            // Debug information
            Console.WriteLine($"Cart ID: {cart.CartId}");
            Console.WriteLine($"Cart Items Count: {cart.CartItems?.Count ?? 0}");

            var viewModel = new CartViewModel
            {
                CartId = cart.CartId,
                CustomerId = cart.CustomerId,
                Items = cart.CartItems?.Select(item => new CartItemViewModel
                {
                    CartItemId = item.CartItemId,
                    AlbumId = item.Id,
                    Title = item.Album?.Title,
                    Artist = item.Album?.Artist,
                    Price = item.Album?.Price ?? 0,
                    Quantity = item.Quantity,
                    Subtotal = (item.Album?.Price ?? 0) * item.Quantity
                }).ToList() ?? new List<CartItemViewModel>(),
                TotalAmount = cart.CartItems?.Sum(item => (item.Album?.Price ?? 0) * item.Quantity) ?? 0
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddToCart(int albumId, int quantity = 1)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);

            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get cart with included items
            var cart = _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Album)
                .FirstOrDefault(c => c.CustomerId == customer.Id);

            if (cart == null)
            {
                cart = new Cart { CustomerId = customer.Id };
                _cartRepository.Add(cart);
                _cartRepository.SaveChanges();
            }

            var albumItem = _albumRepository.Get(albumId);

            if (albumItem == null)
            {
                return NotFound();
            }

            var existingProduct = cart.CartItems.FirstOrDefault(x => x.Id == albumId);

            if (existingProduct == null)
            {
                var newCartItem = new CartItem
                {
                    Id = albumItem.Id,
                    Album = albumItem,
                    Quantity = quantity,
                    AddedAt = DateTime.Now
                };
                cart.CartItems.Add(newCartItem);
            }
            else
            {
                existingProduct.Quantity += quantity;
            }

            if (cart.CartId == 0) // New cart
            {
                _cartRepository.Add(cart);
            }
            else
            {
                _cartRepository.Update(cart);
            }
            _cartRepository.SaveChanges();

            TempData["SuccessMessage"] = $"{quantity} {albumItem.Title} {(quantity > 1 ? "items" : "item")} added to cart!";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult ApplyCoupon(string couponCode)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);

            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Album)
                .FirstOrDefault(c => c.CustomerId == customer.Id);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            var viewModel = new CartViewModel
            {
                CartId = cart.CartId,
                CustomerId = cart.CustomerId,
                Items = cart.CartItems.Select(item => new CartItemViewModel
                {
                    CartItemId = item.CartItemId,
                    AlbumId = item.Id,
                    Title = item.Album?.Title ?? "Unknown",
                    Artist = item.Album?.Artist ?? "Unknown",
                    Price = item.Album?.Price ?? 0,
                    Quantity = item.Quantity,
                    Subtotal = (item.Album?.Price ?? 0) * item.Quantity
                }).ToList(),
                TotalAmount = cart.CartItems.Sum(item => (item.Album?.Price ?? 0) * item.Quantity),
                CouponCode = couponCode
            };

            // 🔥 Here is the coupon logic (you can extend this easily)
            if (!string.IsNullOrEmpty(couponCode))
            {
                switch (couponCode.ToUpper())
                {
                    case "SAVE10":
                        viewModel.DiscountAmount = viewModel.TotalAmount * 0.10m; // 10% off
                        break;
                    case "SAVE20":
                        viewModel.DiscountAmount = viewModel.TotalAmount * 0.20m; // 20% off
                        break;
                    default:
                        TempData["ErrorMessage"] = "Invalid coupon code.";
                        viewModel.CouponCode = string.Empty;
                        viewModel.DiscountAmount = 0;
                        break;
                }
            }

            return View("Index", viewModel);
        }

        public IActionResult Clear()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.CustomerId == customer.Id);

            if (cart != null)
            {
                cart.CartItems.Clear();
                _cartRepository.Update(cart);
                _cartRepository.SaveChanges();
                TempData["SuccessMessage"] = "Cart cleared successfully!";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);

            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = _cartRepository.GetAll().FirstOrDefault(c => c.CustomerId == customer.Id);
            var item = cart?.CartItems.FirstOrDefault(i => i.CartItemId == id);

            if (item == null)
            {
                return NotFound();
            }

            var viewModel = new CartItemViewModel
            {
                CartItemId = item.CartItemId,
                AlbumId = item.Id,
                Title = item.Album.Title,
                Artist = item.Album.Artist,
                Price = item.Album.Price,
                Quantity = item.Quantity,
                Subtotal = item.Album.Price * item.Quantity
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(CartItemViewModel viewModel)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = _cartRepository.GetAll().FirstOrDefault(c => c.CustomerId == customer.Id);
            var item = cart?.CartItems.FirstOrDefault(i => i.CartItemId == viewModel.CartItemId);

            if (item == null)
            {
                return NotFound();
            }

            item.Quantity = viewModel.Quantity;
            _cartRepository.Update(cart);
            _cartRepository.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);

            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Album)
                .FirstOrDefault(c => c.CustomerId == customer.Id);

            var item = cart?.CartItems.FirstOrDefault(i => i.CartItemId == id);

            if (item == null)
            {
                return NotFound();
            }

            var viewModel = new CartItemViewModel
            {
                CartItemId = item.CartItemId,
                AlbumId = item.Id,
                Title = item.Album?.Title ?? "Unknown",
                Artist = item.Album?.Artist ?? "Unknown",
                Price = item.Album?.Price ?? 0,
                Quantity = item.Quantity,
                Subtotal = (item.Album?.Price ?? 0) * item.Quantity
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);

            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.CustomerId == customer.Id);

            var item = cart?.CartItems.FirstOrDefault(i => i.CartItemId == id);

            if (item != null)
            {
                cart.CartItems.Remove(item);
                _cartRepository.Update(cart);
                _cartRepository.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);

            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Album)
                .FirstOrDefault(c => c.CustomerId == customer.Id);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            var viewModel = new CartViewModel
            {
                CartId = cart.CartId,
                CustomerId = cart.CustomerId,
                Items = cart.CartItems.Select(item => new CartItemViewModel
                {
                    CartItemId = item.CartItemId,
                    AlbumId = item.Id,
                    Title = item.Album?.Title ?? "Unknown",
                    Artist = item.Album?.Artist ?? "Unknown",
                    Price = item.Album?.Price ?? 0,
                    Quantity = item.Quantity,
                    Subtotal = (item.Album?.Price ?? 0) * item.Quantity
                }).ToList(),
                TotalAmount = cart.CartItems.Sum(item => (item.Album?.Price ?? 0) * item.Quantity)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);

            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.CustomerId == customer.Id);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            // Process the order here
            // For now, just clear the cart
            cart.CartItems.Clear();
            _cartRepository.Update(cart);
            _cartRepository.SaveChanges();

            TempData["SuccessMessage"] = "Order placed successfully!";
            return RedirectToAction("Index", "Home");

        }
        [HttpGet]
        public IActionResult IncreaseQuantity(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);

            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Album)
                .FirstOrDefault(c => c.CustomerId == customer.Id);

            if (cart == null)
            {
                return RedirectToAction("Index");
            }

            var item = cart.CartItems.FirstOrDefault(i => i.CartItemId == id);
            if (item != null)
            {
                item.Quantity += 1;
                _cartRepository.Update(cart);
                _cartRepository.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult DecreaseQuantity(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);

            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Album)
                .FirstOrDefault(c => c.CustomerId == customer.Id);

            if (cart == null)
            {
                return RedirectToAction("Index");
            }

            var item = cart.CartItems.FirstOrDefault(i => i.CartItemId == id);
            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity -= 1;
                }
                else
                {
                    // If quantity becomes 0, remove the item from cart
                    cart.CartItems.Remove(item);
                }

                _cartRepository.Update(cart);
                _cartRepository.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}