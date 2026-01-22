using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Property
{
    public class CreatePropertyViewModel
    {
        // Basic Info
        [Required(ErrorMessage = "Tên BDS là bắt buộc")]
        [MaxLength(200, ErrorMessage = "Tên không được quá 200 ký tự")]
        [Display(Name = "Tên BĐS")]
        public string Name { get; set; }

        // Location
        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [MaxLength(500, ErrorMessage = "Địa chỉ không được quá 500 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [MaxLength(100)]
        [Display(Name = "Thành phố")]
        public string City { get; set; }

        [MaxLength(100)]
        [Display(Name = "Quận")]
        public string District { get; set; }

        [MaxLength(20)]
        [Display(Name = "Mã bưu điện")]
        public string ZipCode { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude phải từ -90 đến 90")]
        [Display(Name = "Vĩ độ")]
        public decimal? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude phải từ -180 đến 180")]
        [Display(Name = "Kinh độ")]
        public decimal? Longitude { get; set; }

        // Property Type & Specs
        [Required(ErrorMessage = "Loại BDS là bắt buộc")]
        [MaxLength(50)]
        [Display(Name = "Loại BDS")]
        public string PropertyType { get; set; }

        [Required(ErrorMessage = "Số phòng ngủ là bắt buộc")]
        [Range(0, 20, ErrorMessage = "Số phòng ngủ từ 0 đến 20")]
        [Display(Name = "Phòng ngủ")]
        public int Bedrooms { get; set; }

        [Required(ErrorMessage = "Số phòng tắm là bắt buộc")]
        [Range(0, 10, ErrorMessage = "Số phòng tắm từ 0 đến 10")]
        [Display(Name = "Phòng tắm")]
        public int Bathrooms { get; set; }

        [Required(ErrorMessage = "Diện tích là bắt buộc")]
        [Range(1, 100000, ErrorMessage = "Diện tích phải từ 1 đến 100,000 ft²")]
        [Display(Name = "Diện tích (ft²)")]
        public decimal SquareFeet { get; set; }

        // Pricing
        [Required(ErrorMessage = "Giá thuê là bắt buộc")]
        [Range(100000, double.MaxValue, ErrorMessage = "Giá thuê tối thiểu 100,000 VNĐ")]
        [Display(Name = "Giá thuê/tháng")]
        public decimal RentAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền cọc phải >= 0")]
        [Display(Name = "Tiền cọc")]
        public decimal? DepositAmount { get; set; }

        // Description & Details
        [MaxLength(3000, ErrorMessage = "Mô tả không được quá 3000 ký tự")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = "Chưa cập nhật";

        [MaxLength(1000)]
        [Display(Name = "Tiện ích")]
        public string Amenities { get; set; } = "Chưa cập nhật";

        [MaxLength(500)]
        [Display(Name = "Tiện ích đi kèm")]
        public string UtilitiesIncluded { get; set; } = "Chưa cập nhật";

        // Features
        [Display(Name = "Nội thất đầy đủ")]
        public bool IsFurnished { get; set; } = false;

        [Display(Name = "Cho phép thú cưng")]
        public bool PetsAllowed { get; set; } = false;

        [Display(Name = "Có sẵn từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? AvailableFrom { get; set; }

        // Dropdowns (not bound from form)
        public List<SelectListItem> PropertyTypes { get; set; } = new();
        public List<SelectListItem> AvailableLandlords { get; set; } = new();
    }
}