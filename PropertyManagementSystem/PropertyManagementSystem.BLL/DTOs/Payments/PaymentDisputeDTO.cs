namespace PropertyManagementSystem.BLL.DTOs.Payments
{
    public class PaymentDisputeDTO
    {
        public int DisputeId { get; set; }
        public int InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public int RaisedBy { get; set; }
        public string? TenantName { get; set; }
        public string Reason { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "Pending"; 
        public string? Resolution { get; set; }
        public int? ResolvedBy { get; set; }
        public string? AdminName { get; set; } 
        public DateTime? ResolvedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
