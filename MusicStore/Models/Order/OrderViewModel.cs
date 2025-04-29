using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Model.Entities;

namespace MusicStore.Models.Order
{
    public class OrderViewModel
    {
        [Key]
        public int OrderId { get; set; }

        public int CustomerId { get; set; }
        public string UserId { get; set; }

        public string CustomerName { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; }

    }
}
