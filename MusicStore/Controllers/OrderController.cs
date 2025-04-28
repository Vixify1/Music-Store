using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Model.Entities;
using MusicStore.Model.Abstract;
using MusicStore.Models.Order;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using MusicStore.Models.Admin.Customer;
using MusicStore.Models.Admin.Order;
using Microsoft.AspNetCore.Authorization;

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
                return RedirectToAction("Login", "Authentication");
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

            ModelState.Remove("Customer");
            ModelState.Remove("OrderItems");

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

                return RedirectToAction("OrderManagement", "Order");
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

            return RedirectToAction("OrderManagement", "Order");
        }




        // Management Orders method  for admin purpose
        public async Task<IActionResult> OrderManagement(string searchString, string status = "", int page = 1, string sortOrder = "date_desc")
        {
            var pageSize = 10;
            var query = _orderRepository.GetAll()
                .Include(o => o.Customer)
                    .ThenInclude(c => c.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Album)
                .AsQueryable();

            // Apply filtering
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(o => o.OrderId.ToString().Contains(searchString) ||
                                       o.Customer.User.Email.Contains(searchString) ||
                                       o.Customer.User.UserName.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var orderStatus))
            {
                query = query.Where(o => o.Status == orderStatus);
            }

            // Apply sorting
            switch (sortOrder)
            {
                case "date_asc":
                    query = query.OrderBy(o => o.OrderDate);
                    break;
                case "total_desc":
                    query = query.OrderByDescending(o => o.TotalAmount);
                    break;
                case "total_asc":
                    query = query.OrderBy(o => o.TotalAmount);
                    break;
                case "customer":
                    query = query.OrderBy(o => o.Customer.User.UserName);
                    break;
                case "status":
                    query = query.OrderBy(o => o.Status);
                    break;
                default: // date_desc
                    query = query.OrderByDescending(o => o.OrderDate);
                    break;
            }

            // Calculate paging
            var totalOrders = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);

            // Get the current page of orders
            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Get status options for the filter dropdown
            var statusOptions = Enum.GetNames(typeof(OrderStatus)).ToList();

            // Create view model with order data
            var orderViewModels = orders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                CustomerId = o.CustomerId,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                UserId = o.Customer?.User?.Id.ToString(),
                OrderItems = o.OrderItems.Select(oi => new OrderItemViewModel
                {
                    AlbumTitle = oi.Album?.Title ?? "Unknown",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Quantity * oi.UnitPrice
                }).ToList()
            }).ToList();

            // Create the management view model
            var viewModel = new OrderManagementViewModel
            {
                Orders = orderViewModels,
                CurrentPage = page,
                TotalPages = totalPages,
                SearchString = searchString,
                Status = status,
                SortOrder = sortOrder,
                StatusOptions = statusOptions
            };

            return View(viewModel);
        }

    }
}