using System.ComponentModel.DataAnnotations;
using PropertyManagementSystem.BLL.DTOs.Invoice;

namespace PropertyManagementSystem.Web.ViewModels.Payment
{
    public class MakePaymentViewModel
    {
        [Required(ErrorMessage = "Please select an invoice")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid invoice selected")]
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Please enter the payment amount")]
        [Range(1000, double.MaxValue, ErrorMessage = "Minimum payment amount is 1,000 VND")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please select a payment method")]
        [RegularExpression("^(Cash|BankTransfer|CreditCard|Momo|ZaloPay)$", ErrorMessage = "Invalid payment method")]
        public string PaymentMethod { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        public List<InvoiceDto> AvailableInvoices { get; set; } = new();
    }
}
