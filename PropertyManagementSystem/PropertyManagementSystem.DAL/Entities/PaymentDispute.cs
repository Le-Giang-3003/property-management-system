using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class PaymentDispute
    {
        [Key]
        public int DisputeId { get; set; }

        [ForeignKey("Invoice")]
        public int InvoiceId { get; set; }

        [ForeignKey("RaisedByUser")]
        public int RaisedBy { get; set; }

        [Required, MaxLength(100)]
        public string Reason { get; set; }

        [Required, MaxLength(2000)]
        public string Description { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Open"; // Open, UnderReview, Resolved, Rejected

        [MaxLength(2000)]
        public string Resolution { get; set; }

        [ForeignKey("ResolvedByUser")]
        public int? ResolvedBy { get; set; }

        public DateTime? ResolvedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Invoice Invoice { get; set; }
        public User RaisedByUser { get; set; }
        public User ResolvedByUser { get; set; }
    }
}
