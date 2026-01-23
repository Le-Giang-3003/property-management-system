using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Lease
{
    public class CreateLeaseViewModel
    {
        public int ApplicationId { get; set; }

        // Thông tin từ Application (readonly)
        public string? ApplicationNumber { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyAddress { get; set; }
        public string? TenantName { get; set; }
        public string? TenantEmail { get; set; }
        public string? TenantPhone { get; set; }

        [Display(Name = "Ngày bắt đầu thuê")]
        public DateTime StartDate { get; set; }

        public int OriginalStartDay { get; set; } // Để hiển thị cảnh báo

        [Required(ErrorMessage = "Vui lòng chọn thời hạn thuê")]
        [Display(Name = "Thời hạn thuê")]
        public int LeaseDurationMonths { get; set; } = 12;

        public List<SelectListItem> DurationOptions { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "3", Text = "3 tháng" },
            new SelectListItem { Value = "6", Text = "6 tháng" },
            new SelectListItem { Value = "9", Text = "9 tháng" },
            new SelectListItem { Value = "12", Text = "12 tháng (1 năm)", Selected = true },
            new SelectListItem { Value = "18", Text = "18 tháng" },
            new SelectListItem { Value = "24", Text = "24 tháng (2 năm)" },
            new SelectListItem { Value = "36", Text = "36 tháng (3 năm)" }
        };

        [Display(Name = "Ngày kết thúc (dự kiến)")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Ngày thanh toán hàng tháng")]
        public int PaymentDueDay { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiền thuê")]
        [Range(0, double.MaxValue, ErrorMessage = "Tiền thuê phải là số dương")]
        [Display(Name = "Tiền thuê hàng tháng (VNĐ)")]
        public decimal MonthlyRent { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiền cọc")]
        [Range(0, double.MaxValue, ErrorMessage = "Tiền cọc phải là số dương")]
        [Display(Name = "Tiền đặt cọc (VNĐ)")]
        public decimal SecurityDeposit { get; set; }

        [Display(Name = "Điều khoản hợp đồng")]
        public string? Terms { get; set; }

        [Display(Name = "Điều kiện đặc biệt")]
        public string? SpecialConditions { get; set; }

        [Display(Name = "Tự động gia hạn")]
        public bool AutoRenew { get; set; } = false;
    }

    
}
