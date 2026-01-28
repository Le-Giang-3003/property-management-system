using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Property
{
    /// <summary>
    /// DTO for Property Search/Filter - All fields are optional
    /// </summary>
    public class PropertySearchDto
    {
        /// <summary>Search by city or district (optional).</summary>
        [MaxLength(100)]
        public string? City { get; set; }

        /// <summary>Filter by property type: Apartment, House, Condo, Studio, Villa, Office (optional).</summary>
        [MaxLength(50)]
        public string? PropertyType { get; set; }

        /// <summary>Minimum rent (optional).</summary>
        [Range(0, double.MaxValue, ErrorMessage = "Min rent must be >= 0")]
        public decimal? MinRent { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Max rent must be >= 0")]
        public decimal? MaxRent { get; set; }

        [Range(0, 20, ErrorMessage = "Min bedrooms must be 0-20")]
        public int? MinBedrooms { get; set; }

        [Range(0, 10, ErrorMessage = "Min bathrooms must be 0-10")]
        public int? MinBathrooms { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Min area must be >= 0")]
        public decimal? MinSquareFeet { get; set; }

        /// <summary>Filter by status: Available, Rented, Maintenance (optional).</summary>
        [MaxLength(20)]
        public string? Status { get; set; }

        /// <summary>Furnished or not (optional).</summary>
        public bool? IsFurnished { get; set; }

        /// <summary>Pets allowed or not (optional).</summary>
        public bool? PetsAllowed { get; set; }

        /// <summary>Sort by: price_asc, price_desc, date_desc (newest first).</summary>
        [MaxLength(20)]
        public string? SortBy { get; set; } = "date_desc";

        /// <summary>Page number (pagination).</summary>
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        /// <summary>Items per page (pagination).</summary>
        [Range(1, 100)]
        public int PageSize { get; set; } = 20;

        /// <summary>Validation: MaxRent must be >= MinRent.</summary>
        public bool IsValid(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (MinRent.HasValue && MaxRent.HasValue && MinRent > MaxRent)
            {
                errorMessage = "Max rent must be >= min rent";
                return false;
            }

            if (MinBedrooms.HasValue && MinBedrooms < 0)
            {
                errorMessage = "Min bedrooms must be >= 0";
                return false;
            }

            if (MinBathrooms.HasValue && MinBathrooms < 0)
            {
                errorMessage = "Min bathrooms must be >= 0";
                return false;
            }

            if (MinSquareFeet.HasValue && MinSquareFeet < 0)
            {
                errorMessage = "Min area must be >= 0";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check nếu có filter nào được áp dụng
        /// </summary>
        public bool HasFilters()
        {
            return !string.IsNullOrWhiteSpace(City) ||
                   !string.IsNullOrWhiteSpace(PropertyType) ||
                   MinRent.HasValue ||
                   MaxRent.HasValue ||
                   MinBedrooms.HasValue ||
                   MinBathrooms.HasValue ||
                   MinSquareFeet.HasValue ||
                   !string.IsNullOrWhiteSpace(Status) ||
                   IsFurnished.HasValue ||
                   PetsAllowed.HasValue;
        }
    }
}
