using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Schedule
{
    public class RescheduleViewingRequestDto
    {
        [Required]
        public int ViewingId { get; set; }

        [Required]
        public DateTime NewScheduledDate { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}
