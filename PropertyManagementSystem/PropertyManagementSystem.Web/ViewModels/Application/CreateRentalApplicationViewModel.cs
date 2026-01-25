using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Application
{
    public class CreateRentalApplicationViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn bất động sản")]
        [Display(Name = "Bất động sản")]
        public int PropertyId { get; set; }

        // Danh sách properties để hiển thị trong dropdown
        public IEnumerable<SelectListItem>? AvailableProperties { get; set; }

        public string? PropertyName { get; set; }
        public string? PropertyAddress { get; set; }
        public decimal MonthlyRent { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tình trạng việc làm")]
        [Display(Name = "Tình trạng việc làm")]
        public string EmploymentStatus { get; set; }

        [Display(Name = "Tên công ty/Nơi làm việc")]
        [MaxLength(200)]
        public string? Employer { get; set; }

        [Display(Name = "Thu nhập hàng tháng (VNĐ)")]
        [Range(0, double.MaxValue, ErrorMessage = "Thu nhập phải là số dương")]
        public decimal? MonthlyIncome { get; set; }

        [Display(Name = "Địa chỉ nơi ở trước đây")]
        [MaxLength(500)]
        public string? PreviousAddress { get; set; }

        [Display(Name = "Tên chủ nhà trước")]
        [MaxLength(200)]
        public string? PreviousLandlord { get; set; }

        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$",
            ErrorMessage = "Số điện thoại không hợp lệ. VD: 0901234567")]
        [Display(Name = "Số điện thoại chủ nhà trước")]
        [MaxLength(20)]
        public string? PreviousLandlordPhone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số người ở")]
        [Range(1, 20, ErrorMessage = "Số người ở phải từ 1 đến 20")]
        [Display(Name = "Số người ở")]
        public int NumberOfOccupants { get; set; } = 1;

        [Display(Name = "Bạn có nuôi thú cưng không?")]
        public bool HasPets { get; set; } = false;

        [Display(Name = "Thông tin thú cưng")]
        [MaxLength(200)]
        public string? PetDetails { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày dự kiến chuyển vào")]
        [Display(Name = "Ngày dự kiến chuyển vào")]
        [DataType(DataType.Date)]
        public DateTime DesiredMoveInDate { get; set; }

        [Display(Name = "Ghi chú thêm")]
        [MaxLength(2000)]
        public string? AdditionalNotes { get; set; }
    }
}
