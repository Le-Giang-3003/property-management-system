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
            // ✅ Tự động tạo invoices cho các lease Active nhưng chưa có invoice
            await EnsureInvoicesForActiveLeasesAsync(tenantId);

            var invoices = await _invoiceRepository.GetAvailableInvoicesByTenantAsync(tenantId);

            return invoices.Select(MapToDto).ToList();
        }

        // ✅ Tự động tạo invoices cho các lease Active nhưng chưa có invoice
        private async Task EnsureInvoicesForActiveLeasesAsync(int tenantId)
        {
            var activeLeases = await _leaseRepository.GetActiveLeasesForTenantAsync(tenantId);
            var today = DateTime.UtcNow.Date;

            foreach (var lease in activeLeases)
            {
                // Kiểm tra xem đã có invoice nào cho lease này chưa
                var existingInvoices = await _invoiceRepository.GetInvoicesByLeaseIdAsync(lease.LeaseId);
                
                if (!existingInvoices.Any())
                {
                    // Tạo invoice đầu tiên cho tháng hiện tại hoặc tháng bắt đầu của lease
                    var periodStart = lease.StartDate.Date;
                    var periodEnd = periodStart.AddMonths(1);
                    
                    // Chỉ tạo invoice nếu lease đã bắt đầu hoặc sắp bắt đầu trong tháng này
                    if (periodStart <= today && lease.EndDate >= today)
                    {
                        try
                        {
                            await CreateInvoiceFromLeaseAsync(lease.LeaseId, periodStart, periodEnd);
                        }
                        catch (Exception ex)
                        {
                            // Log error nhưng không fail
                            System.Diagnostics.Debug.WriteLine($"Error creating invoice for lease {lease.LeaseId}: {ex.Message}");
                        }
                    }
                }
            }
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
            
            // Tự động set PaidDate khi invoice được thanh toán đầy đủ
            if (dto.Status == "Paid" && dto.RemainingAmount <= 0 && !existing.PaidDate.HasValue)
            {
                existing.PaidDate = DateTime.UtcNow;
            }

            var updated = await _invoiceRepository.UpdateInvoiceAsync(existing);
            return updated == null ? null : MapToDto(updated);
        }

        public async Task<InvoiceDto> CreateInvoiceFromLeaseAsync(int leaseId, DateTime periodStart, DateTime periodEnd)
        {
            var lease = await _leaseRepository.GetByIdAsync(leaseId);
            if (lease == null)
                throw new Exception("Lease not found");

            var amount = lease.MonthlyRent;
            var sercurityDeposit = lease.SecurityDeposit;
            var totalAmount = amount + sercurityDeposit;

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
                TotalAmount = totalAmount,
                PaidAmount = 0,
                RemainingAmount = amount,
                Description = $"Rent for {periodStart:MM/yyyy}",
                Notes = "",
                Status = "Pending",
                BillingMonth = periodStart,
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

        public async Task<InvoiceDto?> CreateFirstInvoiceWithDepositAsync(int leaseId)
        {
            var lease = await _leaseRepository.GetByIdAsync(leaseId);
            if (lease == null)
                throw new Exception("Lease not found");

            if (lease.Status != "Active")
                return null;

            var billingMonth = new DateTime(lease.StartDate.Year, lease.StartDate.Month, 1);

            // Prevent duplicate invoice for the same billing month
            if (await HasInvoiceForMonthAsync(lease.LeaseId, billingMonth))
                return null;

            var now = DateTime.UtcNow;
            var dueDate = now.AddDays(10); // align with monthly invoice rule

            var rentAmount = lease.MonthlyRent;
            var depositAmount = lease.SecurityDeposit;
            var totalAmount = rentAmount + depositAmount;

            var invoice = new Invoice
            {
                LeaseId = lease.LeaseId,
                InvoiceNumber = $"INV-{lease.LeaseId}-{billingMonth:yyyyMM}",
                InvoiceType = depositAmount > 0 ? "Rent+Deposit" : "Rent",
                IssueDate = now,
                DueDate = dueDate,
                Amount = rentAmount,
                TaxAmount = 0,
                DiscountAmount = 0,
                TotalAmount = totalAmount,
                PaidAmount = 0,
                RemainingAmount = totalAmount,
                Description = depositAmount > 0
                    ? $"First payment for {billingMonth:MM/yyyy} (Rent + Deposit)"
                    : $"First payment for {billingMonth:MM/yyyy} (Rent)",
                Notes = depositAmount > 0
                    ? $"Includes: Rent={rentAmount:N0} VND, Deposit={depositAmount:N0} VND"
                    : $"Includes: Rent={rentAmount:N0} VND",
                Status = "Pending",
                BillingMonth = billingMonth,
                CreatedAt = now,
                UpdatedAt = now,
                InvoiceFileUrl = "",
                InvoiceFilePath = "",
                EmailSent = false,
                ReminderCount = 0
            };

            await _invoiceRepository.AddAsync(invoice);

            invoice.Lease = lease;
            return MapToDto(invoice);
        }

        public async Task<List<ActiveLeaseForInvoiceDto>> GetActiveLeasesByLandlordIdAsync(int landlordId)
        {
            var leases = await _leaseRepository.GetByLandlordIdAsync(landlordId);

            return leases
                .Where(l => l.Status == "Active")
                .Select(l => new ActiveLeaseForInvoiceDto
                {
                    LeaseId = l.LeaseId,
                    LeaseNumber = l.LeaseNumber,
                    PropertyName = l.Property?.Name ?? "N/A",
                    PropertyAddress = l.Property?.Address ?? "N/A",
                    TenantName = l.Tenant?.FullName ?? "N/A",
                    TenantEmail = l.Tenant?.Email ?? "N/A",
                    MonthlyRent = l.MonthlyRent,
                    PaymentDueDay = l.PaymentDueDay,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Status = l.Status
                })
                .ToList();
        }

        public async Task<InvoiceDto> CreateInvoiceWithAdditionalAmountAsync(int leaseId, decimal additionalAmount, string? additionalDescription)
        {
            var lease = await _leaseRepository.GetByIdAsync(leaseId);
            if (lease == null)
                throw new Exception("Lease not found");

            if (lease.Status != "Active")
                throw new Exception("Can only create invoice for active leases");

            var now = DateTime.UtcNow;
            var periodStart = new DateTime(now.Year, now.Month, 1);
            var periodEnd = periodStart.AddMonths(1);

            // Calculate due date based on payment due day
            var dueDate = new DateTime(now.Year, now.Month, Math.Min(lease.PaymentDueDay, DateTime.DaysInMonth(now.Year, now.Month)));
            if (dueDate < now)
            {
                // If due date already passed this month, set to next month
                dueDate = dueDate.AddMonths(1);
            }

            var baseAmount = lease.MonthlyRent;
            var totalAmount = baseAmount + additionalAmount;

            var description = $"Rent for {periodStart:MM/yyyy}";
            if (additionalAmount > 0 && !string.IsNullOrWhiteSpace(additionalDescription))
            {
                description += $" + {additionalDescription}";
            }
            else if (additionalAmount > 0)
            {
                description += " + Additional services";
            }

            // Generate invoice number with timestamp to avoid duplicates
            var invoiceNumber = $"INV-{lease.LeaseId}-{now:yyyyMMddHHmmss}";

            var invoice = new Invoice
            {
                LeaseId = lease.LeaseId,
                InvoiceNumber = invoiceNumber,
                InvoiceType = additionalAmount > 0 ? "Rent+Services" : "Rent",
                IssueDate = now,
                DueDate = dueDate,
                Amount = baseAmount,
                TaxAmount = 0,
                DiscountAmount = 0,
                TotalAmount = totalAmount,
                PaidAmount = 0,
                RemainingAmount = totalAmount,
                Description = description,
                Notes = additionalAmount > 0 ? $"Additional services: {additionalAmount:N0} VND" : "",
                Status = "Pending",
                BillingMonth = periodStart,
                CreatedAt = now,
                UpdatedAt = now,
                InvoiceFileUrl = "",
                InvoiceFilePath = "",
                EmailSent = false,
                ReminderCount = 0
            };

            await _invoiceRepository.AddAsync(invoice);

            return MapToDto(invoice);
        }

        public async Task<InvoiceDto?> CreateMonthlyInvoiceAsync(Lease lease, DateTime billingMonth)
        {
            if (lease == null)
                return null;

            if (lease.Status != "Active")
                return null;

            // Check if invoice already exists for this month
            if (await HasInvoiceForMonthAsync(lease.LeaseId, billingMonth))
                return null;

            var now = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(billingMonth.Year, billingMonth.Month, 1);

            // DueDate = IssueDate + 10 days (as per requirement)
            var dueDate = now.AddDays(10);

            var invoiceNumber = $"INV-{lease.LeaseId}-{billingMonth:yyyyMM}";

            var invoice = new Invoice
            {
                LeaseId = lease.LeaseId,
                InvoiceNumber = invoiceNumber,
                InvoiceType = "Rent",
                IssueDate = now,
                DueDate = dueDate,
                Amount = lease.MonthlyRent,
                TaxAmount = 0,
                DiscountAmount = 0,
                TotalAmount = lease.MonthlyRent,
                PaidAmount = 0,
                RemainingAmount = lease.MonthlyRent,
                Description = $"Monthly rent for {billingMonth:MMMM yyyy}",
                Notes = $"Billing period: {firstDayOfMonth:dd/MM/yyyy} - {firstDayOfMonth.AddMonths(1).AddDays(-1):dd/MM/yyyy}",
                Status = "Pending",
                BillingMonth = firstDayOfMonth,
                CreatedAt = now,
                UpdatedAt = now,
                InvoiceFileUrl = "",
                InvoiceFilePath = "",
                EmailSent = false,
                ReminderCount = 0
            };

            await _invoiceRepository.AddAsync(invoice);

            // Reload invoice with Lease navigation property
            invoice.Lease = lease;

            return MapToDto(invoice);
        }

        public async Task<bool> HasInvoiceForMonthAsync(int leaseId, DateTime billingMonth)
        {
            return await _invoiceRepository.HasInvoiceForMonthAsync(leaseId, billingMonth);
        }

        public async Task UpdateOverdueInvoicesAsync()
        {
            var overdueInvoices = await _invoiceRepository.GetOverdueInvoicesAsync();

            foreach (var invoice in overdueInvoices)
            {
                invoice.Status = "Overdue";
                invoice.UpdatedAt = DateTime.UtcNow;
                await _invoiceRepository.UpdateInvoiceAsync(invoice);
            }
        }

        private static InvoiceDto MapToDto(Invoice i)
        {
            return new InvoiceDto
            {
                InvoiceId = i.InvoiceId,
                LeaseId = i.LeaseId,
                TenantId = i.Lease?.TenantId ?? 0,
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
                IsOverdue = (i.Status == "Pending" || i.Status == "Overdue")
                            && i.RemainingAmount > 0
                            && i.DueDate.Date < DateTime.Today,

                Description = i.Description,
                CreatedAt = i.CreatedAt,
                BillingMonth = i.BillingMonth
            };
        }
    }
}
