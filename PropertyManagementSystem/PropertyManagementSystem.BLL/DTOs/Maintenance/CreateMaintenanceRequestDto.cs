using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PropertyManagementSystem.BLL.DTOs.Maintenance
{
    public class CreateMaintenanceRequestDto
    {
        [Required(ErrorMessage = "Property is required")]
        public int PropertyId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [MaxLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [MaxLength(20, ErrorMessage = "Priority cannot exceed 20 characters")]
        public string Priority { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(3000, ErrorMessage = "Description cannot exceed 3000 characters")]
        public string Description { get; set; }

        [MaxLength(500, ErrorMessage = "Location cannot exceed 500 characters")]
        public string Location { get; set; }

        public DateTime? RepairDate { get; set; }

        /// <summary>Preferred start time as "HH:mm" from input type="time". Empty when optional field not filled.</summary>
        [MaxLength(10)]
        public string TimeFrom { get; set; }

        /// <summary>Preferred end time as "HH:mm" from input type="time". Empty when optional field not filled.</summary>
        [MaxLength(10)]
        public string TimeTo { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
