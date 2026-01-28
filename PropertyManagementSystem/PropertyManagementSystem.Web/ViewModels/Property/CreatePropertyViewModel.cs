using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Property
{
    public class CreatePropertyViewModel
    {
        // Basic Info
        [Required(ErrorMessage = "Property name is required")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        [Display(Name = "Property Name")]
        public string Name { get; set; }

        // Location
        [Required(ErrorMessage = "Address is required")]
        [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [MaxLength(100)]
        [Display(Name = "City")]
        public string City { get; set; }

        [MaxLength(100)]
        [Display(Name = "District")]
        public string District { get; set; }

        [MaxLength(20)]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        [Display(Name = "Latitude")]
        public decimal? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        [Display(Name = "Longitude")]
        public decimal? Longitude { get; set; }

        // Property Type & Specs
        [Required(ErrorMessage = "Property type is required")]
        [MaxLength(50)]
        [Display(Name = "Property Type")]
        public string PropertyType { get; set; }

        [Required(ErrorMessage = "Number of bedrooms is required")]
        [Range(0, 20, ErrorMessage = "Bedrooms must be between 0 and 20")]
        [Display(Name = "Bedrooms")]
        public int Bedrooms { get; set; }

        [Required(ErrorMessage = "Number of bathrooms is required")]
        [Range(0, 10, ErrorMessage = "Bathrooms must be between 0 and 10")]
        [Display(Name = "Bathrooms")]
        public int Bathrooms { get; set; }

        [Required(ErrorMessage = "Area is required")]
        [Range(1, 100000, ErrorMessage = "Area must be between 1 and 100,000 sq ft")]
        [Display(Name = "Area (sq ft)")]
        public decimal SquareFeet { get; set; }

        // Pricing
        [Required(ErrorMessage = "Rent price is required")]
        [Range(100000, double.MaxValue, ErrorMessage = "Minimum rent is 100,000 VND")]
        [Display(Name = "Rent/Month")]
        public decimal RentAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Security deposit must be >= 0")]
        [Display(Name = "Security Deposit")]
        public decimal? DepositAmount { get; set; }

        // Description & Details
        [MaxLength(3000, ErrorMessage = "Description cannot exceed 3000 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; } = "Not updated";

        [MaxLength(1000)]
        [Display(Name = "Amenities")]
        public string Amenities { get; set; } = "Not updated";

        [MaxLength(500)]
        [Display(Name = "Utilities Included")]
        public string UtilitiesIncluded { get; set; } = "Not updated";

        // Features
        [Display(Name = "Fully Furnished")]
        public bool IsFurnished { get; set; } = false;

        [Display(Name = "Pets Allowed")]
        public bool PetsAllowed { get; set; } = false;

        [Display(Name = "Available From")]
        [DataType(DataType.Date)]
        public DateTime? AvailableFrom { get; set; }

        // Dropdowns (not bound from form)
        public List<SelectListItem> PropertyTypes { get; set; } = new();
        public List<SelectListItem> AvailableLandlords { get; set; } = new();
    }
}
