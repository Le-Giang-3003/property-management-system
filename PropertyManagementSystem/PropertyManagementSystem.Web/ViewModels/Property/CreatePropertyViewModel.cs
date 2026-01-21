// PropertyManagementSystem.Web/ViewModels/Property/CreatePropertyViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.Web.ViewModels.Property
{
    /// <summary>
    /// Vm for creating a new property.
    /// </summary>
    public class CreatePropertyViewModel
    {
        // Core Info
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required(ErrorMessage = "Tên BDS là bắt buộc")]
        [MaxLength(200, ErrorMessage = "Tên BDS tối đa 200 ký tự")]
        [Display(Name = "Tên bất động sản")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [MaxLength(500, ErrorMessage = "Địa chỉ tối đa 500 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>
        /// The city.
        /// </value>
        [MaxLength(100)]
        [Display(Name = "Thành phố")]
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the district.
        /// </summary>
        /// <value>
        /// The district.
        /// </value>
        [MaxLength(100)]
        [Display(Name = "Quận/Huyện")]
        public string? District { get; set; }

        /// <summary>
        /// Gets or sets the zip code.
        /// </summary>
        /// <value>
        /// The zip code.
        /// </value>
        [MaxLength(20)]
        [Display(Name = "Mã bưu điện")]
        public string? ZipCode { get; set; }

        // Location (Optional)
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>
        /// The latitude.
        /// </value>
        [Column(TypeName = "decimal(10,6)")]
        [Display(Name = "Vĩ độ")]
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>
        /// The longitude.
        /// </value>
        [Column(TypeName = "decimal(10,6)")]
        [Display(Name = "Kinh độ")]
        public decimal? Longitude { get; set; }

        // Property Type & Specs
        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        [Required(ErrorMessage = "Loại BDS là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Loại BDS tối đa 50 ký tự")]
        [Display(Name = "Loại BDS")]
        public string PropertyType { get; set; } = string.Empty; // Apartment, House, etc.

        /// <summary>
        /// Gets or sets the bedrooms.
        /// </summary>
        /// <value>
        /// The bedrooms.
        /// </value>
        [Range(0, 20, ErrorMessage = "Số phòng ngủ từ 0-20")]
        [Display(Name = "Số phòng ngủ")]
        public int Bedrooms { get; set; } = 1;

        /// <summary>
        /// Gets or sets the bathrooms.
        /// </summary>
        /// <value>
        /// The bathrooms.
        /// </value>
        [Range(0, 10, ErrorMessage = "Số phòng tắm từ 0-10")]
        [Display(Name = "Số phòng tắm")]
        public int Bathrooms { get; set; } = 1;

        /// <summary>
        /// Gets or sets the square feet.
        /// </summary>
        /// <value>
        /// The square feet.
        /// </value>
        [Required(ErrorMessage = "Diện tích là bắt buộc")]
        [Range(1, 10000, ErrorMessage = "Diện tích từ 1-10,000 ft²")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Diện tích (ft²)")]
        public decimal SquareFeet { get; set; }

        // Pricing
        /// <summary>
        /// Gets or sets the rent amount.
        /// </summary>
        /// <value>
        /// The rent amount.
        /// </value>
        [Required(ErrorMessage = "Giá thuê là bắt buộc")]
        [Range(100, 50000, ErrorMessage = "Giá thuê từ $100-$50,000")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá thuê/tháng")]
        public decimal RentAmount { get; set; }

        /// <summary>
        /// Gets or sets the deposit amount.
        /// </summary>
        /// <value>
        /// The deposit amount.
        /// </value>
        [Range(0, 50000, ErrorMessage = "Tiền cọc từ $0-$50,000")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tiền cọc (Optional)")]
        public decimal? DepositAmount { get; set; }

        // Description & Amenities
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [MaxLength(3000, ErrorMessage = "Mô tả tối đa 3000 ký tự")]
        [Display(Name = "Mô tả chi tiết")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the amenities.
        /// </summary>
        /// <value>
        /// The amenities.
        /// </value>
        [MaxLength(1000, ErrorMessage = "Tiện ích tối đa 1000 ký tự")]
        [Display(Name = "Tiện ích (JSON format)")]
        public string? Amenities { get; set; } // ["AC", "WiFi", "Parking"]

        /// <summary>
        /// Gets or sets the utilities included.
        /// </summary>
        /// <value>
        /// The utilities included.
        /// </value>
        [MaxLength(500, ErrorMessage = "Tiện ích bao gồm tối đa 500 ký tự")]
        [Display(Name = "Tiện ích bao gồm (JSON)")]
        public string? UtilitiesIncluded { get; set; } // ["Water", "Electricity"]

        // Features
        /// <summary>
        /// Gets or sets a value indicating whether this instance is furnished.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is furnished; otherwise, <c>false</c>.
        /// </value>
        [Display(Name = "Có nội thất")]
        public bool IsFurnished { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [pets allowed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [pets allowed]; otherwise, <c>false</c>.
        /// </value>
        [Display(Name = "Cho phép nuôi thú cưng")]
        public bool PetsAllowed { get; set; }

        // Date
        /// <summary>
        /// Gets or sets the available from.
        /// </summary>
        /// <value>
        /// The available from.
        /// </value>
        [Display(Name = "Có sẵn từ")]
        public DateTime? AvailableFrom { get; set; } = DateTime.UtcNow.AddDays(30);

        // Dropdown Data
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
