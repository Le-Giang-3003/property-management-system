namespace PropertyManagementSystem.BLL.DTOs.Invoice
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public int LeaseId { get; set; }

        public int TenantId { get; set; }
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

    public class ActiveLeaseForInvoiceDto
    {
        public int LeaseId { get; set; }
        public string LeaseNumber { get; set; } = null!;
        public string PropertyName { get; set; } = null!;
        public string PropertyAddress { get; set; } = null!;
        public string TenantName { get; set; } = null!;
        public string TenantEmail { get; set; } = null!;
        public decimal MonthlyRent { get; set; }
        public int PaymentDueDay { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
