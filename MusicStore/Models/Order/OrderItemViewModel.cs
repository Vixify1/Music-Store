namespace MusicStore.Models.Order
{
    public class OrderItemViewModel
    {
        public string AlbumTitle { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}