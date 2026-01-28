using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Maintenance
{
    public class AssignTechnicianDto
    {
        [Required(ErrorMessage = "Request ID is required")]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Technician ID is required")]
        public int TechnicianId { get; set; }
    }
}
