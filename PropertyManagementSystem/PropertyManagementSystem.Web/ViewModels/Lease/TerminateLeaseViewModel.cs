using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Lease
{
    public class TerminateLeaseViewModel
    {
        // THÔNG TIN HỢP ĐỒNG (CHỈ ĐỌC)
        public int LeaseId { get; set; }
        public string LeaseNumber { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyAddress { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MonthlyRent { get; set; }

        // THÔNG TIN HỦY HỢP ĐỒNG (NHẬP)
        [Required(ErrorMessage = "Ngày chấm dứt là bắt buộc")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày chấm dứt hợp đồng")]
        public DateTime TerminationDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Lý do hủy hợp đồng là bắt buộc")]
        [MinLength(10, ErrorMessage = "Lý do phải có ít nhất 10 ký tự")]
        [Display(Name = "Lý do hủy hợp đồng")]
        public string Reason { get; set; } = string.Empty;
    }

}
