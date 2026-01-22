namespace PropertyManagementSystem.BLL.DTOs.Invoice
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public int LeaseId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        public string Status { get; set; } = null!; // Pending / Paid
        public bool IsOverdue { get; set; }         // tính từ DueDate + RemainingAmount
    }

}
