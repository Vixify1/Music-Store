using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Helper;
using MusicStore.Model.Abstract;
using MusicStore.Model.Entities;
using MusicStore.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStore.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IEntitiesRepository<Order> _orderRepository;
        private readonly IEntitiesRepository<OrderItems> _orderItemsRepository;
        private readonly IEntitiesRepository<Cart> _cartRepository;
        private readonly IEntitiesRepository<Customer> _customerRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(
            IEntitiesRepository<Order> orderRepository,
            IEntitiesRepository<OrderItems> orderItemsRepository,
            IEntitiesRepository<Cart> cartRepository,
            IEntitiesRepository<Customer> customerRepository,
            UserManager<ApplicationUser> userManager)
        {
            _orderRepository = orderRepository;
            _orderItemsRepository = orderItemsRepository;
            _cartRepository = cartRepository;
            _customerRepository = customerRepository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);
            if (customer == null)
            {
                return RedirectToAction("Create", "Customer");
            }

            var cart = _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Album)
                .FirstOrDefault(c => c.CustomerId == customer.Id);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
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
        public IActionResult ProcessOrder()
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                return RedirectToAction("Login", "Authorization");
            }

            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);
            if (customer == null)
            {
                return RedirectToAction("Create", "Customer");
            }

            var cart = _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Album)
                .FirstOrDefault(c => c.CustomerId == customer.Id);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }


            // Calculate total amount
            decimal totalAmount = cart.CartItems.Sum(item => item.Quantity * (item.Album?.Price ?? 0));

            // Create new order
            var order = new Order
            {
                CustomerId = customer.Id,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                TotalAmount = totalAmount
            };

            _orderRepository.Add(order);
          

            // Create order items
            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItems
                {
                    OrderId = order.OrderId,
                    AlbumId = cartItem.Id,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Album?.Price ?? 0
                };
                _orderItemsRepository.Add(orderItem);
            }

            // Clear the cart
            cart.CartItems.Clear();
            _cartRepository.Update(cart);

            TempData["SuccessMessage"] = "Order placed successfully!";
            // Instead of redirecting to home, redirect to the payment page
            return RedirectToAction("Create", "Payment", new { orderId = order.OrderId , amount = totalAmount });
            //return RedirectToAction("Index", "Home");
        }

        public IActionResult Confirmation(int orderId)
        {
            var order = _orderRepository.Get(orderId);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
