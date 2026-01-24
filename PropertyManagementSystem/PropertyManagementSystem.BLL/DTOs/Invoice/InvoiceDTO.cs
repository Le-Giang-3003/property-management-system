namespace PropertyManagementSystem.BLL.DTOs.Invoice
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public int LeaseId { get; set; }

        public string InvoiceNumber { get; set; } = null!;
        public string InvoiceType { get; set; } = null!;

        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }

        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        public string Status { get; set; } = null!;
        public bool IsOverdue { get; set; }

        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
