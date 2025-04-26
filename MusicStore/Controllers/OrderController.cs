using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Model.Entities;
using MusicStore.Model.Abstract;
using MusicStore.Models.Order;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace MusicStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly IEntitiesRepository<Order> _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEntitiesRepository<Customer> _customerRepository;

        public OrderController(
            IEntitiesRepository<Order> orderRepository,
            UserManager<ApplicationUser> userManager,
            IEntitiesRepository<Customer> customerRepository)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _customerRepository = customerRepository;
        }

        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.UserId == user.Id);
            if (customer == null)
            {
                return RedirectToAction("Create", "Customer");
            }

            var orders = _orderRepository.GetAll()
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Album)
                .Where(o => o.CustomerId == customer.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var viewModels = orders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
                OrderItems = o.OrderItems.Select(oi => new OrderItemViewModel
                {
                    AlbumTitle = oi.Album?.Title ?? "Unknown",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Quantity * oi.UnitPrice
                }).ToList()
            }).ToList();

            return View(viewModels);
        }

        public IActionResult Details(int id)
        {
            var order = _orderRepository.GetAll()
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Album)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new OrderViewModel
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    AlbumTitle = oi.Album?.Title ?? "Unknown",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Quantity * oi.UnitPrice
                }).ToList()
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                order.CreatedAt = DateTime.UtcNow;
                order.UpdatedAt = DateTime.UtcNow;

                _orderRepository.Add(order);
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = _orderRepository.Get(id.Value);
            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Order order)
        {
            if (id != order.OrderId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    order.UpdatedAt = DateTime.UtcNow;
                    _orderRepository.Update(order);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_orderRepository.GetAll().Any(o => o.OrderId == id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var order = _orderRepository.Get(id.Value);
            if (order == null) return NotFound();

            var viewModel = new OrderViewModel
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var order = _orderRepository.Get(id);
            if (order != null)
            {
                _orderRepository.Remove(order);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}