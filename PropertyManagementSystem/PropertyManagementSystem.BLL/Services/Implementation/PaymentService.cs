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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentDisputeRepository _paymentDisputeRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IInvoiceService invoiceService,
            IUnitOfWork unitofwork,
            IPaymentDisputeRepository paymentDisputeRepository,
            IInvoiceRepository invoiceRepository)
        {
            _paymentRepository = paymentRepository;
            _invoiceService = invoiceService;
            _unitOfWork = unitofwork;
            _paymentDisputeRepository = paymentDisputeRepository;
            _invoiceRepository = invoiceRepository;
        }

        // 1. Lấy danh sách hóa đơn sẵn dùng (Tenant)
        public async Task<List<InvoiceDto>> GetAvailableInvoicesAsync(int tenantId)
        {
            return await _invoiceService.GetAvailableInvoicesByTenantAsync(tenantId);
        }

        // 2. Thực hiện thanh toán (Tenant)
        public async Task<PaymentDto> MakePaymentAsync(int tenantId, MakePaymentRequestDto request)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(request.InvoiceId);
            if (invoice == null) throw new Exception("Hóa đơn không tồn tại");

            if (invoice.TenantId != tenantId)
                throw new Exception("Bạn không có quyền thanh toán hóa đơn này");

            if (request.Amount <= 0)
                throw new Exception("Số tiền thanh toán phải lớn hơn 0");

            if (request.Amount > invoice.RemainingAmount)
                throw new Exception("Số tiền thanh toán vượt quá số tiền còn nợ");

            // Cập nhật Invoice qua Service (đã có logic cập nhật status)
            invoice.PaidAmount += request.Amount;
            invoice.RemainingAmount = invoice.TotalAmount - invoice.PaidAmount;
            invoice.Status = invoice.RemainingAmount <= 0 ? "Paid" : "PartiallyPaid";

            await _invoiceService.UpdateInvoiceAsync(invoice);

            var payment = new Payment
            {
                InvoiceId = invoice.InvoiceId,
                PaymentNumber = $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}",
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
                ProcessedBy = 1
            };

            await _paymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

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

        // 3. Lấy lịch sử thanh toán (Tenant)
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

        // 4. Báo cáo thanh toán (Admin & Tenant)
        public async Task<PaymentReportDto> GetPaymentReportAsync(int tenantId, int roleId, DateTime fromDate, DateTime toDate, string? paymentMethod = null)
        {
            IEnumerable<Payment> allPayments;
            if (roleId == 1) allPayments = await _paymentRepository.GetAllAsync();
            else allPayments = await _paymentRepository.GetByTenantAsync(tenantId);

            var filtered = allPayments
                .Where(p => p.PaymentDate.Date >= fromDate.Date && p.PaymentDate.Date <= toDate.Date)
                .Where(p => string.IsNullOrEmpty(paymentMethod) || p.PaymentMethod == paymentMethod)
                .ToList();

            return new PaymentReportDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                SelectedMethod = paymentMethod,
                TotalPaid = filtered.Sum(p => p.Amount),
                TotalTransactions = filtered.Count,
                TenantName = roleId == 1 ? "Toàn hệ thống" : (filtered.FirstOrDefault()?.Invoice?.Lease?.Tenant?.FullName ?? "Người thuê"),
                Payments = filtered.Select(p => new PaymentDto
                {
                    PaymentId = p.PaymentId,
                    InvoiceId = p.InvoiceId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    PaymentDate = p.PaymentDate,
                    Status = p.Status,
                    TenantName = p.Invoice?.Lease?.Tenant?.FullName ?? "N/A",
                    PropertyName = p.Invoice?.Lease?.Property?.Name ?? "N/A"
                }).ToList()
            };
        }

        // 5. CHỨC NĂNG MỚI: Lấy tất cả khiếu nại (Admin)
        public async Task<List<PaymentDisputeDTO>> GetAllDisputesAsync()
        {
            // Repo đã xử lý Include Invoice.Lease.Tenant
            var disputes = await _paymentDisputeRepository.GetAllAsync();

            return disputes.Select(d => new PaymentDisputeDTO
            {
                DisputeId = d.DisputeId,
                InvoiceId = d.InvoiceId,
                Reason = d.Reason,
                Description = d.Description,
                Status = d.Status,
                CreatedAt = d.CreatedAt,
                TenantName = d.Invoice?.Lease?.Tenant?.FullName ?? "N/A",
                Resolution = d.Resolution,
                ResolvedAt = d.ResolvedAt
            }).ToList();
        }

        // 6. Tạo khiếu nại (Tenant)
        public async Task<bool> RaiseDisputeAsync(int userId, RaiseDisputeDTO dto)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(dto.InvoiceId);
            if (invoice == null) throw new Exception("Không tìm thấy hóa đơn.");

            var dispute = new PaymentDispute
            {
                InvoiceId = dto.InvoiceId,
                RaisedBy = userId,
                Reason = dto.Reason,
                Description = dto.Description,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Resolution = "",
            };

            await _paymentDisputeRepository.AddAsync(dispute);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        // 7. Giải quyết khiếu nại (Admin)
        public async Task<bool> ResolveDisputeAsync(int adminId, ResolveDisputeDTO dto)
        {
            var dispute = await _paymentDisputeRepository.GetByIdAsync(dto.DisputeId);
            if (dispute == null) return false;

            dispute.Status = dto.Status;
            dispute.Resolution = dto.Resolution;
            dispute.ResolvedBy = adminId;
            dispute.ResolvedAt = DateTime.UtcNow;

            await _paymentDisputeRepository.UpdateAsync(dispute);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        // 8. Xử lý hoàn tiền (Admin)
        public async Task<bool> ProcessRefundAsync(int disputeId, int adminId, string note)
        {
            var dispute = await _paymentDisputeRepository.GetByIdAsync(disputeId);
            if (dispute == null || dispute.Status != "Pending") return false;

            var invoice = await _invoiceRepository.GetByIdAsync(dispute.InvoiceId);
            if (invoice == null) return false;

            // 1. Hoàn trả tiền nợ cho Hóa đơn
            invoice.PaidAmount = 0;
            invoice.RemainingAmount = invoice.TotalAmount;
            invoice.Status = "Pending";
            invoice.UpdatedAt = DateTime.UtcNow;

            // 2. Cập nhật trạng thái Khiếu nại
            dispute.Status = "Refunded";
            dispute.Resolution = note;
            dispute.ResolvedBy = adminId;
            dispute.ResolvedAt = DateTime.UtcNow;

            // 3. SỬA LỖI TẠI ĐÂY: Lấy danh sách giao dịch của hóa đơn này
            var payments = await _paymentRepository.GetByInvoiceIdAsync(invoice.InvoiceId);

            foreach (var p in payments)
            {
                p.Status = "Refunded";
            }

            await _invoiceRepository.UpdateInvoiceAsync(invoice);
            await _paymentDisputeRepository.UpdateAsync(dispute);

            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}
