using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities;

public class Invoice
{
    [Key]
    public int InvoiceId { get; set; }

    [ForeignKey("Lease")]
    public int LeaseId { get; set; }

    [Required, MaxLength(50)]
    public string InvoiceNumber { get; set; }

    [Required, MaxLength(50)]
    public string InvoiceType { get; set; } // Rent, Utility, Maintenance, Deposit, Other

    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PaidAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal RemainingAmount { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    [MaxLength(1000)]
    public string Notes { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Paid, Overdue, PartiallyPaid, Cancelled, Disputed

    public DateTime? PaidDate { get; set; }

    [MaxLength(500)]
    public string InvoiceFileUrl { get; set; }

    [MaxLength(500)]
    public string InvoiceFilePath { get; set; }

    public bool EmailSent { get; set; } = false;
    public DateTime? EmailSentDate { get; set; }

    public int ReminderCount { get; set; } = 0;
    public DateTime? LastReminderSent { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Lease Lease { get; set; }
    public ICollection<Payment> Payments { get; set; }
    public ICollection<PaymentDispute> Disputes { get; set; }
}
