using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Lease
{
    public class RenewLeaseViewModel
    {
        // READ-ONLY
        public int LeaseId { get; set; }
        public string LeaseNumber { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyAddress { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal CurrentMonthlyRent { get; set; }
        public decimal CurrentSecurityDeposit { get; set; }

        // INPUT
        [Required(ErrorMessage = "Vui lòng nhập số tháng gia hạn")]
        [Range(1, 36, ErrorMessage = "Thời gian gia hạn từ 1-36 tháng")]
        [Display(Name = "Số tháng gia hạn")]
        public int ExtensionMonths { get; set; } = 12;

        [Display(Name = "Tiền thuê mới (VNĐ/tháng)")]
        [Range(0, double.MaxValue, ErrorMessage = "Tiền thuê phải lớn hơn 0")]
        public decimal? NewMonthlyRent { get; set; }

        [Display(Name = "Đặt cọc mới (VNĐ)")]
        [Range(0, double.MaxValue, ErrorMessage = "Đặt cọc phải lớn hơn 0")]
        public decimal? NewSecurityDeposit { get; set; }

        [Display(Name = "Điều khoản bổ sung")]
        [StringLength(2000, ErrorMessage = "Điều khoản không quá 2000 ký tự")]
        public string? AdditionalTerms { get; set; }

        [Display(Name = "Tự động gia hạn")]
        public bool AutoRenew { get; set; }

        // CALCULATED
        public DateTime NewStartDate => EndDate.AddDays(1);
        public DateTime NewEndDate => NewStartDate.AddMonths(ExtensionMonths);
    }
}
