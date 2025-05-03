namespace MusicStore.Models.Payment
{
    public class PaymentUpdateStatusViewModel
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
    }

}
