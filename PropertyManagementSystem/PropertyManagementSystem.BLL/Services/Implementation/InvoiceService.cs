using PropertyManagementSystem.BLL.DTOs.Invoice;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation


{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILeaseRepository _leaseRepository;

        public  InvoiceService(
            IInvoiceRepository invoiceRepository,
            ILeaseRepository leaseRepository)
        {
            _invoiceRepository = invoiceRepository;
            _leaseRepository = leaseRepository;
        }

        public async Task<List<InvoiceDto>> GetAvailableInvoicesByTenantAsync(int tenantId)
        {
            var invoices = await _invoiceRepository.GetAvailableInvoicesByTenantAsync(tenantId);

            return invoices.Select(MapToDto).ToList();
        }

        public async Task<InvoiceDto?> GetInvoiceByIdAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            return invoice == null ? null : MapToDto(invoice);
        }

        public async Task<InvoiceDto?> UpdateInvoiceAsync(InvoiceDto dto)
        {
            var existing = await _invoiceRepository.GetByIdAsync(dto.InvoiceId);
            if (existing == null)
                return null;

            // Map từ dto sang entity (chỉ những field cho phép sửa)
            existing.DueDate = dto.DueDate;
            existing.TotalAmount = dto.TotalAmount;
            existing.PaidAmount = dto.PaidAmount;
            existing.RemainingAmount = dto.RemainingAmount;
            existing.Status = dto.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _invoiceRepository.UpdateInvoiceAsync(existing);
            return updated == null ? null : MapToDto(updated);
        }

        public async Task<InvoiceDto> CreateInvoiceFromLeaseAsync(int leaseId, DateTime periodStart, DateTime periodEnd)
        {
            var lease = await _leaseRepository.GetByIdAsync(leaseId);
            if (lease == null)
                throw new Exception("Lease không tồn tại");

            var amount = lease.MonthlyRent;

            var issueDate = DateTime.UtcNow;
            var dueDate = new DateTime(periodEnd.Year, periodEnd.Month, lease.PaymentDueDay);

            var invoice = new Invoice
            {
                LeaseId = lease.LeaseId,
                InvoiceNumber = $"INV-{lease.LeaseId}-{periodStart:yyyyMM}",
                InvoiceType = "Rent",
                IssueDate = issueDate,
                DueDate = dueDate,
                Amount = amount,
                TaxAmount = 0,
                DiscountAmount = 0,
                TotalAmount = amount,
                PaidAmount = 0,
                RemainingAmount = amount,
                Description = $"Tiền thuê kỳ {periodStart:MM/yyyy}",
                Notes = "",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                InvoiceFileUrl = "",
                InvoiceFilePath = "",
                EmailSent = false,
                ReminderCount = 0
            };

            await _invoiceRepository.AddAsync(invoice);

            return MapToDto(invoice);
        }

        private static InvoiceDto MapToDto(Invoice i)
        {
            return new InvoiceDto
            {
                InvoiceId = i.InvoiceId,
                LeaseId = i.LeaseId,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceType = i.InvoiceType,
                IssueDate = i.IssueDate,
                DueDate = i.DueDate,

                Amount = i.Amount,
                TaxAmount = i.TaxAmount,
                DiscountAmount = i.DiscountAmount,
                TotalAmount = i.TotalAmount,
                PaidAmount = i.PaidAmount,
                RemainingAmount = i.RemainingAmount,

                Status = i.Status,
                IsOverdue = i.Status == "Pending"
                            && i.RemainingAmount > 0
                            && i.DueDate.Date < DateTime.Today,

                Description = i.Description,
                CreatedAt = i.CreatedAt
            };
        }
    }
}
