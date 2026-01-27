using PropertyManagementSystem.BLL.DTOs.Lease;
using PropertyManagementSystem.BLL.DTOs.Maintenance;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class LeaseService : ILeaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInvoiceService _invoiceService;

        public LeaseService(IUnitOfWork unitOfWork, IInvoiceService invoiceService)
        {
            _unitOfWork = unitOfWork;
            _invoiceService = invoiceService;
        }

        private int CalculatePaymentDueDay(DateTime startDate)
        {
            int day = startDate.Day;
            return day >= 29 ? 28 : day;
        }

        public async Task<Lease> GetLeaseByIdAsync(int id)
        {
            return await _unitOfWork.Leases.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Lease>> GetAllLeasesAsync()
        {
            return await _unitOfWork.Leases.GetAllAsync();
        }

        public async Task<IEnumerable<Lease>> GetLeasesByPropertyIdAsync(int propertyId)
        {
            return await _unitOfWork.Leases.GetByPropertyIdAsync(propertyId);
        }

        public async Task<IEnumerable<Lease>> GetLeasesByTenantIdAsync(int tenantId)
        {
            return await _unitOfWork.Leases.GetByTenantIdAsync(tenantId);
        }

        public async Task<IEnumerable<Lease>> GetLeasesByLandlordIdAsync(int landlordId)
        {
            return await _unitOfWork.Leases.GetByLandlordIdAsync(landlordId);
        }

        public async Task<Lease> CreateLeaseFromApplicationAsync(CreateLeaseDto dto, int createdBy)
        {
            var application = await _unitOfWork.RentalApplications.GetByIdAsync(dto.ApplicationId);
            if (application == null || application.Status != "Approved")
            {
                return null;
            }

            var property = await _unitOfWork.Properties.GetPropertyByIdAsync(application.PropertyId);
            if (property == null || property.Status != "PendingLease")
            {
                return null;
            }

            var existingLease = await _unitOfWork.Leases.GetByApplicationIdAsync(dto.ApplicationId);
            if (existingLease != null)
            {
                return null;
            }

            var leaseNumber = await _unitOfWork.Leases.GenerateLeaseNumberAsync();

            DateTime startDate = application.DesiredMoveInDate;
            DateTime endDate = startDate.AddMonths(dto.LeaseDurationMonths);
            int paymentDueDay = CalculatePaymentDueDay(startDate);

            string terms = dto.Terms;
            if (string.IsNullOrWhiteSpace(terms))
            {
                terms = GenerateDefaultTerms(dto.MonthlyRent, paymentDueDay, startDate);
            }

            var lease = new Lease
            {
                PropertyId = application.PropertyId,
                TenantId = application.ApplicantId,
                ApplicationId = dto.ApplicationId,
                LeaseNumber = leaseNumber,
                StartDate = startDate,
                EndDate = endDate,
                MonthlyRent = dto.MonthlyRent,
                SecurityDeposit = dto.SecurityDeposit,
                PaymentDueDay = paymentDueDay,
                PaymentFrequency = "Monthly",
                Terms = terms,
                SpecialConditions = string.IsNullOrWhiteSpace(dto.SpecialConditions) ? null : dto.SpecialConditions.Trim(),
                AutoRenew = dto.AutoRenew,
                Status = "Draft",
                CreatedAt = DateTime.UtcNow
            };

            var result = await _unitOfWork.Leases.CreateAsync(lease);
            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        public async Task<bool> UpdateLeaseAsync(UpdateLeaseDto dto)
        {
            var lease = await _unitOfWork.Leases.GetByIdAsync(dto.LeaseId);
            if (lease == null)
                return false;

            if (lease.Status != "Draft")
                return false;

            DateTime endDate = dto.StartDate.AddMonths(dto.LeaseDurationMonths);
            int paymentDueDay = CalculatePaymentDueDay(dto.StartDate);

            lease.StartDate = dto.StartDate;
            lease.EndDate = endDate;
            lease.PaymentDueDay = paymentDueDay;
            lease.MonthlyRent = dto.MonthlyRent;
            lease.SecurityDeposit = dto.SecurityDeposit;
            lease.Terms = string.IsNullOrWhiteSpace(dto.Terms) ? null : dto.Terms.Trim();
            lease.SpecialConditions = string.IsNullOrWhiteSpace(dto.SpecialConditions) ? null : dto.SpecialConditions.Trim();
            lease.AutoRenew = dto.AutoRenew;
            lease.UpdatedAt = DateTime.UtcNow;

            var updated = await _unitOfWork.Leases.UpdateAsync(lease);
            if (updated)
            {
                await _unitOfWork.SaveChangesAsync();
            }

            return updated;
        }

        public async Task<IEnumerable<Lease>> GetLeaseHistoryByPropertyIdAsync(int propertyId)
        {
            return await _unitOfWork.Leases.GetLeaseHistoryByPropertyIdAsync(propertyId);
        }

        public async Task<bool> CanCreateLeaseFromApplication(int applicationId)
        {
            var application = await _unitOfWork.RentalApplications.GetByIdAsync(applicationId);
            if (application == null || application.Status != "Approved")
                return false;

            var existingLease = await _unitOfWork.Leases.GetByApplicationIdAsync(applicationId);
            return existingLease == null;
        }

        public async Task<Lease?> GetLeaseByApplicationIdAsync(int applicationId)
        {
            return await _unitOfWork.Leases.GetByApplicationIdAsync(applicationId);
        }

        private string GenerateDefaultTerms(decimal monthlyRent, int paymentDueDay, DateTime startDate)
        {
            string paymentNote = startDate.Day >= 29
                ? $"\n   * Note: Although the lease start date is day {startDate.Day}, the payment due day is adjusted to day 28 for consistency (not all months have 29–31 days)."
                : "";

            return $@"LEASE AGREEMENT TERMS

I. PAYMENT TERMS:
1. Monthly rent: {monthlyRent:N0} VND
2. Payment due date: Day {paymentDueDay} of each month{paymentNote}
3. Late payment fee: 50,000 VND/day after the due date
4. Payment methods: Bank transfer or cash

II. TENANT OBLIGATIONS:
1. Pay rent in full and on time
2. Use the property for its intended purpose
3. Maintain the property and avoid damage
4. Be responsible for electricity, water, and internet bills
5. Provide 30 days' notice if terminating the lease

III. LANDLORD OBLIGATIONS:
1. Deliver the property on the agreed date
2. Ensure the property is in good, safe condition
3. Repair structural defects
4. Do not raise rent during the lease term

IV. TERMINATION:
1. The lease ends automatically at expiry (unless renewed)
2. Early termination requires 30 days' notice
3. Serious breach: The lease may be terminated immediately";
        }

        public async Task<SignLeaseResponseDto> SignLeaseAsync(SignLeaseDto dto)
        {
            var response = new SignLeaseResponseDto
            {
                Success = false,
                Message = "Unable to sign the lease"
            };

            var lease = await _unitOfWork.Leases.GetByIdAsync(dto.LeaseId);
            if (lease == null)
            {
                response.Message = "Lease not found";
                return response;
            }

            if (lease.Status != "Draft")
            {
                response.Message = $"Lease status is '{lease.Status}', cannot sign";
                response.LeaseStatus = lease.Status;
                return response;
            }

            var canSign = await CanUserSignAsync(dto.LeaseId, dto.UserId);
            if (!canSign)
            {
                response.Message = "You do not have permission to sign this lease or have already signed";
                return response;
            }

            var existingSignature = await _unitOfWork.LeaseSignatures
                .GetByLeaseAndUserAsync(dto.LeaseId, dto.UserId);

            if (existingSignature != null)
            {
                response.Message = "You have already signed this lease";
                return response;
            }

            // Create new signature
            var signature = new LeaseSignature
            {
                LeaseId = dto.LeaseId,
                UserId = dto.UserId,
                SignerRole = dto.SignerRole,
                SignatureData = dto.SignatureData,
                SignedAt = DateTime.UtcNow,
                IpAddress = dto.IpAddress
            };

            await _unitOfWork.LeaseSignatures.CreateAsync(signature);

            // Check if all parties have signed
            var hasLandlordSignature = dto.SignerRole == "Landlord" ||
                await _unitOfWork.LeaseSignatures.HasLandlordSignedAsync(dto.LeaseId);
            var hasTenantSignature = dto.SignerRole == "Tenant" ||
                await _unitOfWork.LeaseSignatures.HasTenantSignedAsync(dto.LeaseId);

            if (hasLandlordSignature && hasTenantSignature)
            {
                lease.Status = "Active";
                lease.SignedDate = DateTime.UtcNow;

                // Update property status to Rented
                var property = await _unitOfWork.Properties.GetPropertyByIdAsync(lease.PropertyId);
                if (property != null)
                {
                    property.Status = "Rented";
                    await _unitOfWork.Properties.UpdatePropertyAsync(property);
                }

                await _unitOfWork.Leases.UpdateAsync(lease);

                try
                {
                    var periodStart = lease.StartDate;
                    var periodEnd = lease.StartDate.AddMonths(1);
                    await _invoiceService.CreateInvoiceFromLeaseAsync(lease.LeaseId, periodStart, periodEnd);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error creating initial invoice: {ex.Message}");
                }

                response.Message = "Lease signed successfully! The lease has been activated.";
                response.IsFullySigned = true;
            }
            else
            {
                response.Message = "Lease signed successfully! Waiting for the other party to sign.";
                response.IsFullySigned = false;
            }

            await _unitOfWork.SaveChangesAsync();

            // Map response
            response.Success = true;
            response.LeaseStatus = lease.Status;
            response.SignedDate = lease.SignedDate;
            response.NewSignature = new LeaseSignatureDto
            {
                SignatureId = signature.SignatureId,
                LeaseId = signature.LeaseId,
                UserId = signature.UserId,
                SignerRole = signature.SignerRole,
                SignedAt = signature.SignedAt,
                IpAddress = signature.IpAddress
            };

            return response;
        }


        // ✅ KIỂM TRA LEASE ĐÃ KÝ ĐẦY ĐỦ CHƯA
        public async Task<bool> IsLeaseFullySignedAsync(int leaseId)
        {
            return await _unitOfWork.LeaseSignatures.IsFullySignedAsync(leaseId);
        }

        // ✅ KIỂM TRA USER CÓ THỂ KÝ KHÔNG
        public async Task<bool> CanUserSignAsync(int leaseId, int userId)
        {
            var lease = await _unitOfWork.Leases.GetByIdAsync(leaseId);
            if (lease == null || lease.Status != "Draft")
                return false;

            // Kiểm tra đã ký chưa
            var existingSignature = await _unitOfWork.LeaseSignatures.GetByLeaseAndUserAsync(leaseId, userId);
            if (existingSignature != null)
                return false;

            // Kiểm tra là landlord hoặc tenant
            var property = await _unitOfWork.Properties.GetPropertyByIdAsync(lease.PropertyId);
            return property.LandlordId == userId || lease.TenantId == userId;
        }

        // ✅ LẤY DANH SÁCH CHỮ KÝ
        public async Task<IEnumerable<LeaseSignatureDto>> GetLeaseSignaturesAsync(int leaseId)
        {
            var signatures = await _unitOfWork.LeaseSignatures.GetByLeaseIdAsync(leaseId);

            return signatures.Select(s => new LeaseSignatureDto
            {
                SignatureId = s.SignatureId,
                LeaseId = s.LeaseId,
                UserId = s.UserId,
                UserName = s.User?.FullName ?? "N/A",
                SignerRole = s.SignerRole,
                SignedAt = s.SignedAt,
                IpAddress = s.IpAddress,
                SignatureData = s.SignatureData
            }).ToList();
        }
        // ✅ CẬP NHẬT CÁC LEASE HẾT HẠN
        public async Task UpdateExpiredLeasesAsync()
        {
            var today = DateTime.UtcNow.Date;

            // Lấy tất cả lease active
            var allLeases = await _unitOfWork.Leases.GetAllAsync();
            var expiredLeases = allLeases.Where(l => l.Status == "Active" && l.EndDate < today).ToList();

            foreach (var lease in expiredLeases)
            {
                // Cập nhật lease status
                lease.Status = "Expired";
                await _unitOfWork.Leases.UpdateAsync(lease);

                // Cập nhật property status → Available
                var property = await _unitOfWork.Properties.GetPropertyByIdAsync(lease.PropertyId);
                if (property != null)
                {
                    property.Status = "Available";
                    await _unitOfWork.Properties.UpdatePropertyAsync(property);
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<bool> TerminateLeaseAsync(Lease lease, TerminateLeaseDto terminateDto)
        {
            try
            {
                if (lease == null || terminateDto == null)
                    return false;

                // Chỉ cho phép hủy khi đang Active
                if (lease.Status != "Active")
                    return false;

                /// 1. Cập nhật Lease
                lease.Status = "Terminated";
                lease.TerminatedDate = terminateDto.TerminationDate;
                lease.EndDate = terminateDto.TerminationDate;
                lease.TerminationReason = terminateDto.Reason;


                var terminationNote =
                    $"\n\n--- LEASE TERMINATION ---\n" +
                    $"Termination date: {terminateDto.TerminationDate:dd/MM/yyyy}\n" +
                    $"Reason: {terminateDto.Reason}\n" +
                    $"Recorded at: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

                lease.SpecialConditions = string.IsNullOrEmpty(lease.SpecialConditions)
                    ? terminationNote
                    : lease.SpecialConditions + terminationNote;

                // 2. Cập nhật Property sau, qua repository riêng
                var property = await _unitOfWork.Properties.GetPropertyByIdAsync(lease.PropertyId);
                if (property != null)
                {
                    property.Status = "Available";
                    property.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.Properties.UpdatePropertyAsync(property);
                    await _unitOfWork.SaveChangesAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        // PropertyManagementSystem.BLL.Services.Implementation/LeaseService.cs
        public async Task<bool> CanRenewLeaseAsync(int leaseId)
        {
            var lease = await _unitOfWork.Leases.GetByIdAsync(leaseId);

            if (lease == null)
                return false;

            return lease.Status == "Active";
        }

        public async Task<IEnumerable<Lease>> GetRenewableLeasesAsync()
        {
            return await _unitOfWork.Leases.GetRenewableLeasesAsync(30);
        }

        public async Task<RenewLeaseResponseDto> RenewLeaseAsync(RenewLeaseDto dto, int renewedBy)
        {
            var response = new RenewLeaseResponseDto
            {
                Success = false,
                Message = "Unable to renew the lease"
            };

            var oldLease = await _unitOfWork.Leases.GetLeaseWithDetailsAsync(dto.LeaseId);

            if (oldLease == null)
            {
                response.Message = "Lease not found";
                return response;
            }

            if (oldLease.Status != "Active")
            {
                response.Message = "Only Active leases can be renewed";
                return response;
            }

            if (dto.ExtensionMonths < 1 || dto.ExtensionMonths > 36)
            {
                response.Message = "Extension period must be between 1 and 36 months";
                return response;
            }

            try
            {
                var newLeaseNumber = await _unitOfWork.Leases.GenerateLeaseNumberAsync();

                DateTime newStartDate = oldLease.EndDate.AddDays(1);
                DateTime newEndDate = newStartDate.AddMonths(dto.ExtensionMonths);
                int paymentDueDay = oldLease.PaymentDueDay;

                decimal monthlyRent = dto.NewMonthlyRent ?? oldLease.MonthlyRent;
                decimal securityDeposit = dto.NewSecurityDeposit ?? oldLease.SecurityDeposit;

                string renewalNote = $"\n\n--- RENEWAL FROM LEASE {oldLease.LeaseNumber} ---\n" +
                                   $"Original lease: {oldLease.LeaseNumber}\n" +
                                   $"Previous term: {oldLease.StartDate:dd/MM/yyyy} - {oldLease.EndDate:dd/MM/yyyy}\n" +
                                   $"Renewed at: {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}\n";

                if (dto.NewMonthlyRent.HasValue)
                {
                    renewalNote += $"Previous rent: {oldLease.MonthlyRent:N0} VND → New: {monthlyRent:N0} VND\n";
                }

                string terms = oldLease.Terms + renewalNote;

                if (!string.IsNullOrWhiteSpace(dto.AdditionalTerms))
                {
                    terms += $"\n--- ADDITIONAL TERMS ---\n{dto.AdditionalTerms.Trim()}\n";
                }

                var newLease = new Lease
                {
                    PropertyId = oldLease.PropertyId,
                    TenantId = oldLease.TenantId,
                    ApplicationId = oldLease.ApplicationId,
                    LeaseNumber = newLeaseNumber,
                    StartDate = newStartDate,
                    EndDate = newEndDate,
                    MonthlyRent = monthlyRent,
                    SecurityDeposit = securityDeposit,
                    PaymentDueDay = paymentDueDay,
                    PaymentFrequency = "Monthly",
                    Terms = terms,
                    SpecialConditions = dto.AdditionalTerms,
                    AutoRenew = dto.AutoRenew,
                    Status = "Draft",
                    PreviousLeaseId = oldLease.LeaseId,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _unitOfWork.Leases.CreateAsync(newLease);

                oldLease.Status = "Renewed";
                oldLease.SpecialConditions = string.IsNullOrEmpty(oldLease.SpecialConditions)
                    ? $"Renewed by lease {newLeaseNumber}"
                    : oldLease.SpecialConditions + $"\n\nRenewed by lease {newLeaseNumber}";

                await _unitOfWork.Leases.UpdateAsync(oldLease);
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Message = "Lease renewed successfully! Please sign the new lease.";
                response.NewLeaseId = result.LeaseId;
                response.NewStartDate = newStartDate;
                response.NewEndDate = newEndDate;

                return response;
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi gia hạn: {ex.Message}";
                return response;
            }
        }
        public async Task<List<PropertySelectDto>> GetTenantActivePropertiesAsync(int tenantId)
        {
            var activeLeases = await _unitOfWork.Leases.GetByTenantIdAsync(tenantId);

            // Include Active, Draft, and PendingSignature leases (tenant can create maintenance requests for properties they have leases for, even if not fully signed yet)
            var result = activeLeases
                .Where(l => l.Status == "Active" || l.Status == "Draft" || l.Status == "PendingSignature")
                .Select(l => new PropertySelectDto
                {
                    PropertyId = l.PropertyId,
                    Name = l.Property?.Name ?? "N/A",
                    Address = l.Property?.Address ?? "N/A"
                })
                .Distinct()
                .ToList();

            return result;
        }
        public async Task<bool> ValidateTenantPropertyAccessAsync(int tenantId, int propertyId)
        {
            var leases = await _unitOfWork.Leases.GetByTenantIdAsync(tenantId);
            return leases.Any(l => l.PropertyId == propertyId && l.Status == "Active");
        }

    }
}
