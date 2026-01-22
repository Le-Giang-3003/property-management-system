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

        public async Task<PaymentDto> MakePaymentAsync(int tenantId, MakePaymentRequestDto request)
        {
            // Load invoice
            var invoice = await _invoiceService.GetInvoiceByIdAsync(request.InvoiceId);
            if (invoice == null)
                throw new Exception("Hóa đơn không tồn tại");

            // TODO: Check invoice có thuộc tenant này không

            // Cập nhật hóa đơn
            invoice.PaidAmount += request.Amount;
            invoice.RemainingAmount = invoice.TotalAmount - invoice.PaidAmount;

            if (invoice.RemainingAmount <= 0)
            {
                invoice.Status = "Paid";
                invoice.PaidDate = DateTime.UtcNow;
            }
            else if (invoice.PaidAmount > 0)
            {
                invoice.Status = "PartiallyPaid";
            }

            // Tạo payment
            var payment = new Payment
            {
                InvoiceId = invoice.InvoiceId,
                PaymentNumber = $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}",
                Amount = request.Amount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = request.PaymentMethod,
                Status = "Confirmed",
                Notes = request.Notes,
                ConfirmedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);

            return new PaymentDto
            {
                PaymentId = payment.PaymentId,
                InvoiceId = payment.InvoiceId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                PaymentDate = payment.PaymentDate,
                Status = payment.Status
            };
        }

        public async Task<List<Invoice>> GetAvailableInvoicesAsync(int tenantId)
        {
            return await _invoiceService.GetAvailableInvoicesByTenantAsync(tenantId);
        }
    }
}
