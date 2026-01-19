using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels
{
    /// <summary>
    /// View model for creating a new property.
    /// </summary>
    public class CreatePropertyViewModel
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [Required]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        [Required]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>
        /// The city.
        /// </value>
        [Required]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the district.
        /// </summary>
        /// <value>
        /// The district.
        /// </value>
        public string? District { get; set; }

        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        [Required]
        public string? PropertyType { get; set; } // e.g., Apartment, House, Condo

        /// <summary>
        /// Gets or sets the base rent price.
        /// </summary>
        /// <value>
        /// The base rent price.
        /// </value>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal BaseRentPrice { get; set; }

        /// <summary>
        /// Gets or sets the area.
        /// </summary>
        /// <value>
        /// The area.
        /// </value>
        [Range(0.01, double.MaxValue)]
        public decimal? Area { get; set; }
        /// <summary>
        /// Gets or sets the landlord identifier.
        /// </summary>
        /// <value>
        /// The landlord identifier.
        /// </value>
        [Required]
        public int LandlordId { get; set; }

        // Dữ liệu phụ cho UI (dropdown lists, v.v.)
        /// <summary>
        /// Gets or sets the available landlords.
        /// </summary>
        /// <value>
        /// The available landlords.
        /// </value>
        public List<SelectListItem>? AvailableLandlords { get; set; }
        /// <summary>
        /// Gets or sets the property types.
        /// </summary>
        /// <value>
        /// The property types.
        /// </value>
        public List<SelectListItem>? PropertyTypes { get; set; }
    }
}
