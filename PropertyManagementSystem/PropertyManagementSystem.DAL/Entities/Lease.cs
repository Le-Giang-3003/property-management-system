using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class Lease
    {
        [Key]
        public int LeaseId { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }

        [ForeignKey("Tenant")]
        public int TenantId { get; set; }

        [ForeignKey("RentalApplication")]
        public int? ApplicationId { get; set; }

        [Required, MaxLength(50)]
        public string LeaseNumber { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyRent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SecurityDeposit { get; set; }

        public int PaymentDueDay { get; set; } = 1;

        [MaxLength(20)]
        public string PaymentFrequency { get; set; } = "Monthly";

        [MaxLength(3000)]
        public string Terms { get; set; }

        [MaxLength(500)]
        public string SpecialConditions { get; set; }

        [MaxLength(500)]
        public string ContractFileUrl { get; set; }

        [MaxLength(500)]
        public string ContractFilePath { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Draft"; // Draft, PendingSignature, Active, Expired, Terminated, Renewed

        public DateTime? SignedDate { get; set; }

        [MaxLength(1000)]
        public string TerminationReason { get; set; }

        public DateTime? TerminatedDate { get; set; }

        public bool AutoRenew { get; set; } = false;

        [ForeignKey("PreviousLease")]
        public int? PreviousLeaseId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Property Property { get; set; }
        public User Tenant { get; set; }
        public RentalApplication RentalApplication { get; set; }
        public Lease PreviousLease { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<LeaseSignature> Signatures { get; set; }
        public ICollection<ChatRoom> ChatRooms { get; set; }
    }
}
