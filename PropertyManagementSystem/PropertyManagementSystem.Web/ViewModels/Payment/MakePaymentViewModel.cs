using System.ComponentModel.DataAnnotations;
using PropertyManagementSystem.BLL.DTOs.Invoice;

namespace PropertyManagementSystem.Web.ViewModels.Payment
{
    public class MakePaymentViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn hóa đơn")]
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public string PaymentMethod { get; set; } = null!;

        public string? Notes { get; set; }

        public List<InvoiceDto> AvailableInvoices { get; set; } = new();
    }
}
