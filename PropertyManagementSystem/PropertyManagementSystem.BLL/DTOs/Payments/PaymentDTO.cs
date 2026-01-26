namespace PropertyManagementSystem.BLL.DTOs.Payments
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }

        public string InvoiceNumber { get; set; } = null!;
        public decimal InvoiceTotalAmount { get; set; }
        public decimal InvoicePaidAmount { get; set; }
        public decimal InvoiceRemainingAmount { get; set; }

        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = null!;
    }

}
