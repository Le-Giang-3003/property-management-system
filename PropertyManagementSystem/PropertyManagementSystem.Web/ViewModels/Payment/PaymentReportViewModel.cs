using System.ComponentModel.DataAnnotations;
using PropertyManagementSystem.BLL.DTOs.Payments;

namespace PropertyManagementSystem.Web.ViewModels.Payment
{
    public class PaymentReportViewModel
    {
        [Display(Name = "Từ ngày")]
        public DateTime FromDate { get; set; }

        [Display(Name = "Đến ngày")]
        public DateTime ToDate { get; set; }

        public decimal TotalAmountPaid { get; set; }
        public int TransactionCount { get; set; }

        public List<PaymentDto> PaymentHistory { get; set; } = new();
        public string? TenantName { get; set; }
    }
}
