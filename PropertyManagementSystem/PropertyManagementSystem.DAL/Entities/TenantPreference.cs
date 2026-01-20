using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class TenantPreference
    {
        [Key]
        public int PreferenceId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinBudget { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxBudget { get; set; }

        [MaxLength(500)]
        public string PreferredLocations { get; set; } // JSON

        [MaxLength(200)]
        public string PreferredPropertyTypes { get; set; } // JSON

        public int? MinBedrooms { get; set; }
        public int? MinBathrooms { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinSquareFeet { get; set; }

        [MaxLength(500)]
        public string RequiredAmenities { get; set; } // JSON

        public bool PetsRequired { get; set; } = false;
        public bool FurnishedRequired { get; set; } = false;

        public DateTime? MoveInDate { get; set; }

        [MaxLength(2000)]
        public string AdditionalNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public User User { get; set; }
    }

}
