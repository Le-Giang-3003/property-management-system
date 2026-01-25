using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Maintenance
{
    public class RejectMaintenanceRequestDto
    {
        [Required(ErrorMessage = "Request ID is required")]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Rejection reason is required")]
        [MaxLength(2000, ErrorMessage = "Reason cannot exceed 2000 characters")]
        public string Reason { get; set; }
    }
}
