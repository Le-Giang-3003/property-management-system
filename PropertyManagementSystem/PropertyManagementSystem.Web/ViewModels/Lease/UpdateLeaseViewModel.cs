using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Lease
{
    public class UpdateLeaseViewModel
    {
        public int LeaseId { get; set; }
        public string LeaseNumber { get; set; }

        [Required]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; }

        public int OriginalStartDay { get; set; }

        [Required]
        [Range(1, 120)]
        [Display(Name = "Thời hạn thuê (tháng)")]
        public int LeaseDurationMonths { get; set; }

        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Ngày thanh toán")]
        public int PaymentDueDay { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Tiền thuê hàng tháng (VNĐ)")]
        public decimal MonthlyRent { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Tiền đặt cọc (VNĐ)")]
        public decimal SecurityDeposit { get; set; }

        [Display(Name = "Điều khoản hợp đồng")]
        public string? Terms { get; set; }

        [Display(Name = "Điều kiện đặc biệt")]
        public string? SpecialConditions { get; set; }

        [Display(Name = "Tự động gia hạn")]
        public bool AutoRenew { get; set; }

        // Thông tin readonly
        public string? PropertyName { get; set; }
        public string? TenantName { get; set; }
        public string? Status { get; set; }
    }
}
