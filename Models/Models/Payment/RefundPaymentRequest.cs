namespace Models.Models.Payment
{
    public class RefundPaymentRequest
    {
        public double Price { get; set; }
        public string PaymentId { get; set; }
    }
}
