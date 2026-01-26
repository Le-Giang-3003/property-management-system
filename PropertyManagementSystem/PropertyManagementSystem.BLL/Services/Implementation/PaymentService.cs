using PropertyManagementSystem.BLL.DTOs.Invoice;
using PropertyManagementSystem.BLL.DTOs.Payments;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvoiceService _invoiceService;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IInvoiceService invoiceService)
        {
            _paymentRepository = paymentRepository;
            _invoiceService = invoiceService;
        }

        public async Task<List<InvoiceDto>> GetAvailableInvoicesAsync(int tenantId)
        {
            return await _invoiceService.GetAvailableInvoicesByTenantAsync(tenantId);
        }

        public async Task<PaymentDto> MakePaymentAsync(int tenantId, MakePaymentRequestDto request)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(request.InvoiceId);
            if (invoice == null)
                throw new Exception("Invoice not found.");

            // 1. Check if invoice belongs to this tenant
            if (invoice.TenantId != tenantId)
                throw new Exception("You don't have permission to pay this invoice.");

            // 2. Check invoice status - prevent payment on already paid or invalid invoices
            if (invoice.Status == "Paid")
                throw new Exception("This invoice has already been paid.");
            if (invoice.Status == "Cancelled")
                throw new Exception("This invoice has been cancelled.");
            if (invoice.Status == "Disputed")
                throw new Exception("This invoice is under dispute and cannot be paid.");

            // 3. Validate amount
            if (request.Amount <= 0)
                throw new Exception("Payment amount must be greater than 0.");

            if (request.Amount > invoice.RemainingAmount)
                throw new Exception($"Payment amount exceeds the remaining balance of {invoice.RemainingAmount:N0}.");

            // 4. Update invoice
            invoice.PaidAmount += request.Amount;
            invoice.RemainingAmount = invoice.TotalAmount - invoice.PaidAmount;

            if (invoice.RemainingAmount <= 0)
            {
                invoice.Status = "Paid";
            }
            else if (invoice.PaidAmount > 0)
            {
                invoice.Status = "PartiallyPaid";
            }

            await _invoiceService.UpdateInvoiceAsync(invoice);

            // 5. Create payment record
            var payment = new Payment
            {
                InvoiceId = invoice.InvoiceId,
                PaymentNumber = $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}",
                Amount = request.Amount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = request.PaymentMethod,
                Status = "Confirmed",
                Notes = request.Notes ?? string.Empty,
                ConfirmedAt = DateTime.UtcNow,

                AccountNumber = "DEMO-ACCOUNT",
                BankName = "DEMO-BANK",
                CreatedAt = DateTime.UtcNow,
                TransactionReference = Guid.NewGuid().ToString(),
                ReceiptFilePath = "N/A",
                ReceiptFileUrl = "N/A",
                ProcessedBy = tenantId // Use actual tenant ID instead of hardcoded value
            };

            await _paymentRepository.AddAsync(payment);

            return MapToDto(payment, invoice);
        }

        public async Task<List<PaymentDto>> GetPaymentHistoryAsync(int tenantId)
        {
            var payments = await _paymentRepository.GetByTenantAsync(tenantId);
            if (payments == null)
                return new List<PaymentDto>();

            return payments.Select(p => MapToDto(p, null)).ToList();
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);
            if (payment == null)
                return null;

            return MapToDto(payment, null);
        }

        private static PaymentDto MapToDto(Payment payment, InvoiceDto? invoiceDto)
        {
            return new PaymentDto
            {
                PaymentId = payment.PaymentId,
                InvoiceId = payment.InvoiceId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod ?? string.Empty,
                PaymentDate = payment.PaymentDate,
                Status = payment.Status ?? "Unknown",
                // Invoice information - get from payment's navigation property if invoiceDto is null
                InvoiceNumber = invoiceDto?.InvoiceNumber ?? payment.Invoice?.InvoiceNumber ?? $"INV-{payment.InvoiceId}",
                InvoiceTotalAmount = invoiceDto?.TotalAmount ?? payment.Invoice?.TotalAmount ?? 0,
                InvoicePaidAmount = invoiceDto?.PaidAmount ?? payment.Invoice?.PaidAmount ?? 0,
                InvoiceRemainingAmount = invoiceDto?.RemainingAmount ?? payment.Invoice?.RemainingAmount ?? 0
            };
        }
    }
}
