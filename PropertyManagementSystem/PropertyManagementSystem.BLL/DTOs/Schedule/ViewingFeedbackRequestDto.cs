using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Schedule
{
    public class ViewingFeedbackRequestDto
    {
        [Required]
        public int ViewingId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Feedback { get; set; }
    }
}
