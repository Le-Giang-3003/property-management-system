using System.ComponentModel.DataAnnotations;
using PropertyManagementSystem.BLL.DTOs.Invoice;

namespace PropertyManagementSystem.Web.ViewModels.Payment
{
    public class MakePaymentViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn hóa đơn cần thanh toán")]
        [Range(1, int.MaxValue, ErrorMessage = "Hóa đơn chọn không hợp lệ")]
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        // Thay đổi Range tối thiểu thành 1,000 vì hầu hết ngân hàng VN không hỗ trợ chuyển dưới mức này
        [Range(1000, double.MaxValue, ErrorMessage = "Số tiền thanh toán tối thiểu là 1,000 VNĐ")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [RegularExpression("^(Cash|BankTransfer|CreditCard)$", ErrorMessage = "Phương thức thanh toán không hợp lệ")]
        public string PaymentMethod { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Ghi chú không được dài quá 500 ký tự")]
        public string? Notes { get; set; }
        public List<InvoiceDto> AvailableInvoices { get; set; } = new();
    }
}
