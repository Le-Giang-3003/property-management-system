using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Maintenance
{
    public class CompleteMaintenanceDto
    {
        [Required(ErrorMessage = "Request ID is required")]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Actual cost is required")]
        public decimal ActualCost { get; set; }

        [MaxLength(2000, ErrorMessage = "Resolution notes cannot exceed 2000 characters")]
        public string ResolutionNotes { get; set; }
    }
}
