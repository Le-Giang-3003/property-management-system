using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class Refund
    {
        [Key]
        public int RefundId { get; set; }

        [ForeignKey("Payment")]
        public int PaymentId { get; set; }

        [Required, MaxLength(50)]
        public string RefundNumber { get; set; } // REF-2024-0001

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required, MaxLength(500)]
        public string Reason { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Completed, Rejected

        [ForeignKey("ProcessedByUser")]
        public int? ProcessedBy { get; set; }

        public DateTime? ProcessedAt { get; set; }

        [MaxLength(100)]
        public string TransactionReference { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Payment Payment { get; set; }
        public User ProcessedByUser { get; set; }
    }
<<<<<<< HEAD
=======

>>>>>>> 7864dd8da4821481c77672150503091864b776b9
}
