namespace MusicStore.Models.Wishlist
{
    public class WishlistCheckoutViewModel
    {
        public List<WishlistItemViewModel> Items { get; set; } = new List<WishlistItemViewModel>();
        public decimal TotalAmount { get; set; }

        //public string ShippingAddress { get; set; }
        //public string PaymentMethod { get; set; }
    }

    public class WishlistItemViewModel
    {
        public int WishlistId { get; set; }
        public int AlbumId { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public decimal Price { get; set; }
    }
}
