using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Schedule
{
    public class ConfirmViewingRequestDto
    {
        [Required]
        public int ViewingId { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        [MaxLength(1000)]
        public string? LandlordNotes { get; set; }

        [MaxLength(1000)]
        [Url]
        public string? MeetingLink { get; set; }
    }
}
