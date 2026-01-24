using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Property
{
    /// <summary>
    /// DTO for Property Search/Filter - All fields are optional
    /// </summary>
    public class PropertySearchDto
    {
        /// <summary>
        /// Tìm theo thành phố hoặc quận (optional)
        /// </summary>
        [MaxLength(100)]
        public string? City { get; set; }

        /// <summary>
        /// Lọc theo loại BDS: Apartment, House, Condo, Studio, Villa, Office (optional)
        /// </summary>
        [MaxLength(50)]
        public string? PropertyType { get; set; }

        /// <summary>
        /// Giá thuê tối thiểu (optional)
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Giá thuê tối thiểu phải >= 0")]
        public decimal? MinRent { get; set; }

        /// <summary>
        /// Giá thuê tối đa (optional)
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Giá thuê tối đa phải >= 0")]
        public decimal? MaxRent { get; set; }

        /// <summary>
        /// Lọc theo số phòng ngủ tối thiểu (optional)
        /// </summary>
        [Range(0, 20, ErrorMessage = "Số phòng ngủ phải từ 0-20")]
        public int? MinBedrooms { get; set; }

        /// <summary>
        /// Lọc theo số phòng tắm tối thiểu (optional)
        /// </summary>
        [Range(0, 10, ErrorMessage = "Số phòng tắm phải từ 0-10")]
        public int? MinBathrooms { get; set; }

        /// <summary>
        /// Lọc theo diện tích tối thiểu (optional)
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Diện tích phải >= 0")]
        public decimal? MinSquareFeet { get; set; }

        /// <summary>
        /// Lọc theo trạng thái: Available, Rented, Maintenance (optional)
        /// </summary>
        [MaxLength(20)]
        public string? Status { get; set; }

        /// <summary>
        /// Có nội thất hay không (optional)
        /// </summary>
        public bool? IsFurnished { get; set; }

        /// <summary>
        /// Cho phép thú cưng hay không (optional)
        /// </summary>
        public bool? PetsAllowed { get; set; }

        /// <summary>
        /// Sắp xếp theo: price_asc, price_desc, date_desc (newest first)
        /// </summary>
        [MaxLength(20)]
        public string? SortBy { get; set; } = "date_desc";

        /// <summary>
        /// Số trang (cho pagination)
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Số items mỗi trang (cho pagination)
        /// </summary>
        [Range(1, 100)]
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Validation: MaxRent phải >= MinRent
        /// </summary>
        public bool IsValid(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (MinRent.HasValue && MaxRent.HasValue && MinRent > MaxRent)
            {
                errorMessage = "Giá thuê tối đa phải >= giá thuê tối thiểu";
                return false;
            }

            if (MinBedrooms.HasValue && MinBedrooms < 0)
            {
                errorMessage = "Số phòng ngủ phải >= 0";
                return false;
            }

            if (MinBathrooms.HasValue && MinBathrooms < 0)
            {
                errorMessage = "Số phòng tắm phải >= 0";
                return false;
            }

            if (MinSquareFeet.HasValue && MinSquareFeet < 0)
            {
                errorMessage = "Diện tích phải >= 0";
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
