using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using MusicStore.Model.Context;
using MusicStore.Model.Entities;
using MusicStore.Model.Abstract;
using MusicStore.Models.Payment;

namespace MusicStore.Controllers
{
    [Authorize] // Require authentication for all actions in this controller
    public class PaymentController : Controller
    {
        private readonly IEntitiesRepository<Payment> _paymentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEntitiesRepository<Customer> _customerRepository;
        private readonly IEntitiesRepository<Order> _orderRepository;

        public PaymentController(IEntitiesRepository<Payment> paymentRepository, IEntitiesRepository<Customer> customerRepository, UserManager<ApplicationUser> userManager, IEntitiesRepository<Order> orderRepository)
        {
            _paymentRepository = paymentRepository;
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

        // GET: Payment/Index  
        public async Task<IActionResult> Index()
        {
            var customerId = await GetCurrentCustomerIdAsync();
            if (customerId == null)
                return RedirectToAction("Create", "Customer");

            var payments = await _paymentRepository.GetAll()
                .Include(p => p.Order)
                .Where(p => p.Order.CustomerId == customerId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return View(payments);
        }

        // GET: Payment/Details/5  
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerId = await GetCurrentCustomerIdAsync();
            if (customerId == null)
                return RedirectToAction("Create", "Customer");

            var payment = await _paymentRepository.GetAll()
                .Include(p => p.Order)
                .ThenInclude(o => o.Customer)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.PaymentId == id && p.Order.CustomerId == customerId);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payment/Create/5 (5 is the orderId) // need to check with xhuljano db since problem with UserId, create an order there , because the Album can be a problem || the album id -> in orderitem should be albumID
        [HttpGet("Payment/Create/{orderId}")]
        public async Task<IActionResult> Create(int? orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }

            var customerId = await GetCurrentCustomerIdAsync();
            if (customerId == null)
                return RedirectToAction("Create", "Customer");

            var order = await _orderRepository.GetAll()
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(od => od.Album)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.CustomerId == customerId);

            if (order == null)
            {
                return NotFound();
            }

            // Check if payment already exists for this order
            var existingPayment = await _paymentRepository.GetAll().FirstOrDefaultAsync(p => p.OrderId == orderId);

            if (existingPayment != null)
            {
                TempData["Error"] = "A payment already exists for this order.";
                return RedirectToAction("Details", "Order", new { id = orderId });
            }

            var payment = new Payment
            {
                OrderId = order.OrderId,
                PaymentDate = DateTime.Now,
                PaymentStatus = "Pending",
                PaymentMethod = "Credit Card", // Default payment method
                TransactionId = Guid.NewGuid().ToString()
            };

            // Pass information to the view
            ViewBag.OrderTotal = order.TotalAmount;
            ViewBag.OrderId = order.OrderId;
            ViewBag.OrderItems = order.OrderItems;
            ViewBag.Customer = order.Customer;

            return View(payment);
        }
        [HttpPost("Payment/Process")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(int orderId, string paymentMethod, string cardNumber, string cardName, string expiryDate, string cvv)
        {
            var customerId = await GetCurrentCustomerIdAsync();
            if (customerId == null)
                return RedirectToAction("Create", "Customer");

            var order = await _orderRepository.GetAll()
                .Include(o => o.Customer).
                Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.CustomerId == customerId);

            if (order == null)
            {
                TempData["Error"] = "Order not found or you don't have permission to access it.";
                return RedirectToAction("Index", "Order");
            }

            if (string.IsNullOrEmpty(paymentMethod))
            {
                TempData["Error"] = "Payment method is required.";
                return RedirectToAction("Create", new { orderId });
            }

            if (paymentMethod == "Credit Card")
            {
                if (string.IsNullOrEmpty(cardNumber) || string.IsNullOrEmpty(cardName) ||
                    string.IsNullOrEmpty(expiryDate) || string.IsNullOrEmpty(cvv))
                {
                    TempData["Error"] = "All card details are required.";
                    return RedirectToAction("Create", new { orderId });
                }

            }

            try
            {
                
                var payment = new Payment
                {
                    OrderId = orderId,
                    PaymentMethod = paymentMethod,
                    PaymentDate = DateTime.Now,
                    PaymentStatus = "Completed",
                    TransactionId = Guid.NewGuid().ToString()
                };

                _paymentRepository.Add(payment);
                
                // After succesfull payment adding change the status of the order to completed 
                order.Status = OrderStatus.Completed;
                _orderRepository.Update(order);


                TempData["Success"] = "Payment processed successfully!";
                return RedirectToAction("Confirmation", new { id = payment.PaymentId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error processing payment: " + ex.Message;
                return RedirectToAction("Create", new { orderId });
            }
        }


        // GET: Payment/Confirmation/5 //nice
        public async Task<IActionResult> Confirmation(int id)
        {
            var customerId = await GetCurrentCustomerIdAsync();
            if (customerId == null)
                return RedirectToAction("Create", "Customer");

            var payment = await _paymentRepository.GetAll()
                .Include(p => p.Order)
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(od => od.Album)
                .FirstOrDefaultAsync(p => p.PaymentId == id && p.Order.CustomerId == customerId);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        //// GET: Payment/Cancel/5  //nice
        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            var customerId = await GetCurrentCustomerIdAsync();
            if (customerId == null)
                return RedirectToAction("Create", "Customer");

            var payment = await _paymentRepository.GetAll()
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.PaymentId == id && p.Order.CustomerId == customerId);

            if (payment == null)
            {
                return NotFound();
            }

            if (payment.PaymentStatus != "Pending")
            {
                TempData["Error"] = "Payment cannot be cancelled as it has already been processed.";
                return RedirectToAction("Details", "Order", new { id = payment.OrderId });
            }

            var viewModel = new PaymentCancelViewModel
            {
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                PaymentStatus = payment.PaymentStatus
            };

            return View(viewModel);
        }

        // POST: Payment/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var customerId = await GetCurrentCustomerIdAsync();
            if (customerId == null)
                return RedirectToAction("Create", "Customer");

            var payment = await _paymentRepository.GetAll()
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.PaymentId == id && p.Order.CustomerId == customerId);

            if (payment == null)
            {
                return NotFound();
            }

            if (payment.PaymentStatus != "Pending")
            {
                TempData["Error"] = "Payment cannot be cancelled as it has already been processed.";
            }
            else
            {
                payment.PaymentStatus = "Cancelled";
                _paymentRepository.Update(payment);
                TempData["Success"] = "Payment cancelled successfully.";
            }

            return RedirectToAction("Details", "Order", new { id = payment.OrderId });
        }



        // ADMIN: View all payments
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Admin()
        {
            var payments = await _paymentRepository.GetAll()
                .Include(p => p.Order)
                .ThenInclude(o => o.Customer)
                .ThenInclude(c => c.User)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new PaymentUpdateStatusViewModel
                {
                    PaymentId = p.PaymentId,
                    OrderId = p.OrderId,
                    CustomerName = p.Order.Customer.User.FullName,
                    TotalAmount = p.Order.TotalAmount,
                    PaymentStatus = p.PaymentStatus,
                    PaymentDate = p.PaymentDate
                })
                .ToListAsync();

            return View(payments);
        }



        // Creating a new method  because I am using customerId for the consumer to see their payments , but obviously admin has another Id but it should see all the payment in database
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminPaymentDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _paymentRepository.GetAll()
                .Include(p => p.Order)
                .ThenInclude(o => o.Customer)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.OrderId == id); // Use OrderId here

            if (payment == null)
            {
                return NotFound();
            }



            return View(payment);
        }
        // ADMIN: GET UpdateStatus form
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var payment = await _paymentRepository.GetAll()
                .Include(p => p.Order)
                 .ThenInclude(o => o.Customer)
                 .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.OrderId == id);

            if (payment == null)
            {
                return NotFound();
            }

            // Populate the ViewModel
            var viewModel = new PaymentUpdateStatusViewModel
            {
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                PaymentStatus = payment.PaymentStatus, // This will be used in the View
                CustomerName = payment.Order.Customer.User.FullName,
                TotalAmount = payment.Order.TotalAmount,
                PaymentDate = payment.PaymentDate
            };

            // Populate the ViewBag with current payment status for the dropdown
            ViewBag.SelectedStatus = payment.PaymentStatus;

            return View(viewModel);
        }

        // ADMIN: POST UpdateStatus
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var payment = await _paymentRepository.GetAll()
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
            {
                return NotFound();
            }

            payment.PaymentStatus = status;
            _paymentRepository.Update(payment);

            TempData["Success"] = "Payment status updated successfully.";
            return RedirectToAction("Admin");
        }

        // ADMIN: GET Delete confirmation page
        [Authorize(Roles = "Admin")]   //->>>>>>>>>>> This is a problem when cancel 
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var payment = await _paymentRepository.GetAll()
                .Include(p => p.Order)
                .ThenInclude(o => o.Customer)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
            {
                return NotFound();
            }

            // Populate the ViewModel
            var viewModel = new PaymentUpdateStatusViewModel
            {
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                PaymentStatus = payment.PaymentStatus,
                CustomerName = payment.Order.Customer.User.FullName,
                TotalAmount = payment.Order.TotalAmount,
                PaymentDate = payment.PaymentDate
            };

            return View(viewModel);
        }

        // ADMIN: POST Delete confirmation
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _paymentRepository.GetAsync(id);

            if (payment == null)
            {
                return NotFound();
            }

            _paymentRepository.Remove(payment);

            TempData["Success"] = "Payment successfully deleted.";
            return RedirectToAction("Admin");
        }


    }
}