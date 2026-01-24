using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Maintenance
{
    public class RateMaintenanceDto
    {
        [Required(ErrorMessage = "Request ID is required")]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "Feedback cannot exceed 1000 characters")]
        public string TenantFeedback { get; set; }
    }
}
