using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class PropertyViewing
    {
        [Key]
        public int ViewingId { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }

        [ForeignKey("Tenant")]
        public int? RequestedBy { get; set; } // Nullable cho Guest

        [MaxLength(100)]
        public string? GuestName { get; set; }

        [MaxLength(100)]
        public string? GuestEmail { get; set; }

        [MaxLength(20)]
        public string? GuestPhone { get; set; }

        public DateTime RequestedDate { get; set; }

        public DateTime? PreferredDate1 { get; set; }
        public DateTime? PreferredDate2 { get; set; }
        public DateTime? PreferredDate3 { get; set; }

        public DateTime? ScheduledDate { get; set; }

        [MaxLength(1000)]
        public string? TenantNotes { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Requested"; // Requested, Confirmed, Completed, Cancelled, Rejected, Rescheduled

        [MaxLength(1000)]
        public string? LandlordNotes { get; set; }

        [MaxLength(1000)]
        public string? MeetingLink { get; set; }

        public bool IsVirtualViewing { get; set; } = false;

        public DateTime? CompletedDate { get; set; }

        public int? Rating { get; set; }

        [MaxLength(1000)]
        public string? Feedback { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Property Property { get; set; }
        public User Tenant { get; set; }
    }
}
