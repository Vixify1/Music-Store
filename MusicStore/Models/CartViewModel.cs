using System.Collections.Generic;
using MusicStore.Model.Entities;

namespace MusicStore.Models
{
    public class CartViewModel
    {
        /// <summary>
        /// The list of items in the shopping cart
        /// </summary>
        public List<CartItem> CartItems { get; set; }

        /// <summary>
        /// The total price of all items in the cart
        /// </summary>
        public decimal CartTotal { get; set; }

        /// <summary>
        /// The ID of the cart
        /// </summary>
        public int CartId { get; set; }

        /// <summary>
        /// The ID of the customer who owns the cart
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// The list of items in the shopping cart
        /// </summary>
        public List<CartItemViewModel> Items { get; set; }

        /// <summary>
        /// The total amount of the cart
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// The number of items in the cart
        /// </summary>
        public int ItemCount => CartItems?.Count ?? 0;

        /// <summary>
        /// Flag indicating if the cart is empty
        /// </summary>
        public bool IsEmpty => ItemCount == 0;

        public string CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal => TotalAmount - DiscountAmount;

        /// <summary>
        /// Default constructor initializes the CartItems list
        /// </summary>
        public CartViewModel()
        {
            CartItems = new List<CartItem>();
            CartTotal = 0;
            Items = new List<CartItemViewModel>();
        }
    }

    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int AlbumId { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}