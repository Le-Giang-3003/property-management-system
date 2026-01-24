using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class MaintenanceRequest
    {
        [Key]
        public int RequestId { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }

        [ForeignKey("Tenant")]
        public int RequestedBy { get; set; }

        [ForeignKey("Technician")]
        public int? AssignedTo { get; set; }

        [Required, MaxLength(50)]
        public string RequestNumber { get; set; }

        [Required, MaxLength(100)]
        public string Category { get; set; } // Plumbing, Electrical, HVAC, Appliance, Carpentry, Painting, Other

        [Required, MaxLength(20)]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Emergency

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [Required, MaxLength(3000)]
        public string Description { get; set; }

        [MaxLength(500)]
        public string Location { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Cancelled/Rejected

        public DateTime RequestDate { get; set; } = DateTime.UtcNow; // When the request was created

        public DateOnly? AssignedDate { get; set; }
        public DateTime? RepairDate { get; set; } // The date tenant wants the repair
        public TimeOnly? TimeFrom { get; set; } // The time tenant wants start
        public TimeOnly? TimeTo { get; set; } // The time tenant wants end

        public DateTime? CompletedDate { get; set; }
        public DateTime? ClosedDate { get; set; }

        [ForeignKey("ClosedByUser")]
        public int? ClosedBy { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? EstimatedCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ActualCost { get; set; }

        [MaxLength(2000)]
        public string ResolutionNotes { get; set; }

        public int? Rating { get; set; }

        [MaxLength(1000)]
        public string TenantFeedback { get; set; }

        [MaxLength(20)]
        public string TechnicianStatus { get; set; } // "", Accepted, Rejected, Finished

        // Navigation
        public Property Property { get; set; }
        public User Tenant { get; set; }
        public User Technician { get; set; }
        public User ClosedByUser { get; set; }
        public ICollection<MaintenanceImage> Images { get; set; }
        public ICollection<MaintenanceComment> Comments { get; set; }
    }
}
