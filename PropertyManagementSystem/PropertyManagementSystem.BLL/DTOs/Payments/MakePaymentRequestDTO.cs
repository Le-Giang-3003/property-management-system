using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Payments
{
    public class MakePaymentRequestDto
    {
        [Required(ErrorMessage = "Please select an invoice to pay.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid invoice ID.")]
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Please enter the payment amount.")]
        [Range(1000, double.MaxValue, ErrorMessage = "Minimum payment amount is 1,000 VND.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please select a payment method.")]
        [RegularExpression("^(Cash|BankTransfer|CreditCard)$",
            ErrorMessage = "Invalid payment method (only Cash, BankTransfer, CreditCard accepted).")]
        public string PaymentMethod { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}
