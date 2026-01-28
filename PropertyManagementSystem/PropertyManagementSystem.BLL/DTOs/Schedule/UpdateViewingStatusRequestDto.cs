using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Schedule
{
    public class UpdateViewingStatusRequestDto
    {
        [Required]
        public int ViewingId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty; // Confirmed, Cancelled, Rejected, Completed, Rescheduled

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}
