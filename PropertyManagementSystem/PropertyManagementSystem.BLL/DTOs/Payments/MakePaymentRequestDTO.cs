namespace PropertyManagementSystem.BLL.DTOs.Payments
{
    public class MakePaymentRequestDto
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? Notes { get; set; }
    }

}
