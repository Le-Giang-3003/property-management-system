using System.ComponentModel.DataAnnotations;
using PropertyManagementSystem.BLL.DTOs.Payments;

namespace PropertyManagementSystem.Web.ViewModels.Payment
{
    public class PaymentReportViewModel
    {
        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)] // Giúp trình duyệt hiển thị bộ chọn ngày chuẩn
        public DateTime FromDate { get; set; }

        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        // THÊM TRƯỜNG NÀY ĐỂ LỌC THEO PHƯƠNG THỨC
        [Display(Name = "Phương thức thanh toán")]
        public string? SelectedPaymentMethod { get; set; }

        public decimal TotalAmountPaid { get; set; }
        public int TransactionCount { get; set; }

        public List<PaymentDto> PaymentHistory { get; set; } = new();
        public string? TenantName { get; set; }
    }
}
