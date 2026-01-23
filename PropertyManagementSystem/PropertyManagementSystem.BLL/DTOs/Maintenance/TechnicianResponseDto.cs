using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Maintenance
{
    public class TechnicianResponseDto
    {
        [Required(ErrorMessage = "Request ID is required")]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } // Accepted or Rejected

        public decimal? EstimatedCost { get; set; }

        [MaxLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters")]
        public string Notes { get; set; }
    }
}
