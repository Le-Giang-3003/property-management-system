using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class RentalApplication
    {
        [Key]
        public int ApplicationId { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }

        [ForeignKey("Applicant")]
        public int ApplicantId { get; set; }

        [Required, MaxLength(50)]
        public string ApplicationNumber { get; set; } // APP-2024-0001

        [MaxLength(100)]
        public string EmploymentStatus { get; set; }

        [MaxLength(200)]
        public string Employer { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MonthlyIncome { get; set; }

        [MaxLength(500)]
        public string PreviousAddress { get; set; }

        [MaxLength(200)]
        public string PreviousLandlord { get; set; }

        [MaxLength(20)]
        public string PreviousLandlordPhone { get; set; }

        public int NumberOfOccupants { get; set; } = 1;

        public bool HasPets { get; set; } = false;

        [MaxLength(200)]
        public string PetDetails { get; set; }

        public DateTime DesiredMoveInDate { get; set; }

        [MaxLength(2000)]
        public string AdditionalNotes { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, UnderReview, Approved, Rejected, Withdrawn

        [MaxLength(1000)]
        public string RejectionReason { get; set; }

        [ForeignKey("ReviewedByUser")]
        public int? ReviewedBy { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Property Property { get; set; }
        public User Applicant { get; set; }
        public User ReviewedByUser { get; set; }
        public Lease Lease { get; set; }
        public AITenantScore TenantScore { get; set; }
    }
}
