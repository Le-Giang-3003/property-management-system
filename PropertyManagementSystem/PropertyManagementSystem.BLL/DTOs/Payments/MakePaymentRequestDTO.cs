using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Payments
{
    public class MakePaymentRequestDto
    {
        [Required(ErrorMessage = "Vui lòng chọn hóa đơn cần thanh toán.")]
        [Range(1, int.MaxValue, ErrorMessage = "Mã hóa đơn không hợp lệ.")]
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tiền thanh toán.")]
        [Range(1000, double.MaxValue, ErrorMessage = "Số tiền thanh toán tối thiểu là 1.000 VNĐ.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
        [RegularExpression("^(Cash|BankTransfer|CreditCard)$",
            ErrorMessage = "Phương thức thanh toán không hợp lệ (Chỉ chấp nhận: Cash, BankTransfer, CreditCard).")]
        public string PaymentMethod { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
        public string? Notes { get; set; }
    }
}
