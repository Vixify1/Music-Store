public class CheckoutViewModel
{
    public List<CartItem> CartItems { get; set; }
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; }
    public string PaymentDetails { get; set; }
}
