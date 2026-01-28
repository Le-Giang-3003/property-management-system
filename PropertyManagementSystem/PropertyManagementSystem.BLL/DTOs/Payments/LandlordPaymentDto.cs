namespace PropertyManagementSystem.BLL.DTOs.Payments
{
    public class LandlordPaymentDto
    {
        public int PaymentId { get; set; }
        public string PaymentNumber { get; set; } = null!;
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        
        // Tenant Information
        public int TenantId { get; set; }
        public string TenantName { get; set; } = null!;
        
        // Property Information
        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = null!;
        public string PropertyAddress { get; set; } = null!;
        
        // Payment Information
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public DateTime PaymentDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = null!;
        
        // Invoice Information
        public decimal InvoiceTotalAmount { get; set; }
        public decimal InvoicePaidAmount { get; set; }
        public decimal InvoiceRemainingAmount { get; set; }
    }
}
