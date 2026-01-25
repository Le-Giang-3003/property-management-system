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
                throw new Exception("Hóa đơn không tồn tại");

            // 1. Kiểm tra hóa đơn có thuộc về tenant này không
            if (invoice.TenantId != tenantId)
                throw new Exception("Bạn không có quyền thanh toán hóa đơn này");

            // 2. Kiểm tra số tiền
            if (request.Amount <= 0)
                throw new Exception("Số tiền thanh toán phải lớn hơn 0");

            if (request.Amount > invoice.RemainingAmount)
                throw new Exception("Số tiền thanh toán vượt quá số tiền còn nợ");

            // 3. Cập nhật invoice (ở mức DTO)
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

            // 4. Tạo bản ghi Payment
            var payment = new Payment
            {
                InvoiceId = invoice.InvoiceId,
                PaymentNumber = $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}",
                Amount = request.Amount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = request.PaymentMethod,
                Status = "Confirmed",
                Notes = request.Notes,
                ConfirmedAt = DateTime.UtcNow,

                AccountNumber = "DEMO-ACCOUNT",
                BankName = "DEMO-BANK",
                CreatedAt = DateTime.UtcNow,
                TransactionReference = Guid.NewGuid().ToString(),
                ReceiptFilePath = "N/A",
                ReceiptFileUrl = "N/A",
                ProcessedBy = 1
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
        public async Task<List<PaymentDto>> GetPaymentHistoryAsync(int tenantId)
        {
            var payments = await _paymentRepository.GetByTenantAsync(tenantId);
            return payments.Select(p => new PaymentDto
            {
                PaymentId = p.PaymentId,
                InvoiceId = p.InvoiceId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                PaymentDate = p.PaymentDate,
                Status = p.Status
            }).ToList();
        }

    }
}
