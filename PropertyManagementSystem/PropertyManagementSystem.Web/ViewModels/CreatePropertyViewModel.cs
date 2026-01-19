using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels
{
    public class CreatePropertyViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        public string? District { get; set; }

        [Required]
        public string? PropertyType { get; set; } // e.g., Apartment, House, Condo

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal BaseRentPrice { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? Area { get; set; }
        [Required]
        public int LandlordId { get; set; }

        // Dữ liệu phụ cho UI (dropdown lists, v.v.)
        public List<SelectListItem>? AvailableLandlords { get; set; }
        public List<SelectListItem>? PropertyTypes { get; set; }
    }
}
