using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.BLL.DTOs.Lease;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class LeaseService : ILeaseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LeaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                ? $"\n   * Lưu ý: Mặc dù ngày bắt đầu hợp đồng là ngày {startDate.Day}, ngày thanh toán được điều chỉnh về ngày 28 để đảm bảo tính nhất quán (do không phải tháng nào cũng có đủ 29-31 ngày)."
                : "";

            return $@"ĐIỀU KHOẢN HỢP ĐỒNG THUÊ NHÀ

I. ĐIỀU KHOẢN THANH TOÁN:
1. Tiền thuê hàng tháng: {monthlyRent:N0} VNĐ
2. Ngày thanh toán: Ngày {paymentDueDay} hàng tháng{paymentNote}
3. Phạt trễ hạn: 50,000 VNĐ/ngày sau ngày đến hạn
4. Hình thức thanh toán: Chuyển khoản hoặc tiền mặt

II. NGHĨA VỤ BÊN THUÊ:
1. Thanh toán tiền thuê đầy đủ và đúng hạn
2. Sử dụng nhà đúng mục đích
3. Bảo quản tài sản, không làm hư hỏng
4. Chịu trách nhiệm về các hóa đơn điện, nước, internet
5. Thông báo trước 30 ngày nếu muốn chấm dứt hợp đồng

III. NGHĨA VỤ BÊN CHO THUÊ:
1. Giao nhà đúng thời gian cam kết
2. Đảm bảo nhà ở tình trạng tốt, an toàn
3. Sửa chữa các hư hỏng do lỗi kết cấu
4. Không tăng giá thuê trong thời hạn hợp đồng

IV. ĐIỀU KHOẢN CHẤM DỨT:
1. Hợp đồng tự động chấm dứt khi hết hạn (trừ khi có gia hạn)
2. Chấm dứt sớm phải thông báo trước 30 ngày
3. Vi phạm nghiêm trọng: Hợp đồng có thể bị chấm dứt ngay lập tức";
        }

        // ✅ KÝ HỢP ĐỒNG
        public async Task<SignLeaseResponseDto> SignLeaseAsync(SignLeaseDto dto)
        {
            var response = new SignLeaseResponseDto
            {
                Success = false,
                Message = "Không thể ký hợp đồng"
            };

            // Validate lease tồn tại
            var lease = await _unitOfWork.Leases.GetByIdAsync(dto.LeaseId);
            if (lease == null)
            {
                response.Message = "Không tìm thấy hợp đồng";
                return response;
            }

            // Validate lease status
            if (lease.Status != "Draft")
            {
                response.Message = $"Hợp đồng đang ở trạng thái '{lease.Status}', không thể ký";
                response.LeaseStatus = lease.Status;
                return response;
            }

            // Kiểm tra user có quyền ký không
            var canSign = await CanUserSignAsync(dto.LeaseId, dto.UserId);
            if (!canSign)
            {
                response.Message = "Bạn không có quyền ký hợp đồng này hoặc đã ký rồi";
                return response;
            }

            // Kiểm tra user đã ký chưa
            var existingSignature = await _unitOfWork.LeaseSignatures
                .GetByLeaseAndUserAsync(dto.LeaseId, dto.UserId);

            if (existingSignature != null)
            {
                response.Message = "Bạn đã ký hợp đồng này rồi";
                return response;
            }

            // Tạo signature mới
            var signature = new LeaseSignature
            {
                LeaseId = dto.LeaseId,
                UserId = dto.UserId,
                SignerRole = dto.SignerRole,
                SignatureData = dto.SignatureData,
                SignedAt = DateTime.UtcNow,
                IpAddress = dto.IpAddress
            };

            // Lưu signature
            await _unitOfWork.LeaseSignatures.CreateAsync(signature);

            // Kiểm tra xem đã ký đầy đủ chưa
            var hasLandlordSignature = dto.SignerRole == "Landlord" || 
                await _unitOfWork.LeaseSignatures.HasLandlordSignedAsync(dto.LeaseId);
            var hasTenantSignature = dto.SignerRole == "Tenant" || 
                await _unitOfWork.LeaseSignatures.HasTenantSignedAsync(dto.LeaseId);

            // Nếu cả 2 đã ký → Cập nhật status
            if (hasLandlordSignature && hasTenantSignature)
            {
                lease.Status = "Active";
                lease.SignedDate = DateTime.UtcNow;

                // Cập nhật Property Status → "Rented"
                var property = await _unitOfWork.Properties.GetPropertyByIdAsync(lease.PropertyId);
                if (property != null)
                {
                    property.Status = "Rented";
                    await _unitOfWork.Properties.UpdatePropertyAsync(property);
                }

                await _unitOfWork.Leases.UpdateAsync(lease);

                response.Message = "Ký hợp đồng thành công! Hợp đồng đã được kích hoạt.";
                response.IsFullySigned = true;
            }
            else
        {
                response.Message = "Ký hợp đồng thành công! Đang chờ bên còn lại ký.";
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
                    $"\n\n--- HỦY HỢP ĐỒNG ---\n" +
                    $"Ngày hủy: {terminateDto.TerminationDate:dd/MM/yyyy}\n" +
                    $"Lý do: {terminateDto.Reason}\n" +
                    $"Ghi nhận lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

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
                Message = "Không thể gia hạn hợp đồng"
            };

            var oldLease = await _unitOfWork.Leases.GetLeaseWithDetailsAsync(dto.LeaseId);

            if (oldLease == null)
            {
                response.Message = "Không tìm thấy hợp đồng";
                return response;
            }

            if (oldLease.Status != "Active")
            {
                response.Message = "Chỉ có thể gia hạn hợp đồng đang Active";
                return response;
            }

            if (dto.ExtensionMonths < 1 || dto.ExtensionMonths > 36)
            {
                response.Message = "Thời gian gia hạn phải từ 1 đến 36 tháng";
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

                string renewalNote = $"\n\n--- GIA HẠN TỪ HỢP ĐỒNG {oldLease.LeaseNumber} ---\n" +
                                   $"Hợp đồng gốc: {oldLease.LeaseNumber}\n" +
                                   $"Kỳ hạn cũ: {oldLease.StartDate:dd/MM/yyyy} - {oldLease.EndDate:dd/MM/yyyy}\n" +
                                   $"Gia hạn lúc: {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}\n";

                if (dto.NewMonthlyRent.HasValue)
                {
                    renewalNote += $"Tiền thuê cũ: {oldLease.MonthlyRent:N0} VNĐ → Mới: {monthlyRent:N0} VNĐ\n";
                }

                string terms = oldLease.Terms + renewalNote;

                if (!string.IsNullOrWhiteSpace(dto.AdditionalTerms))
                {
                    terms += $"\n--- ĐIỀU KHOẢN BỔ SUNG ---\n{dto.AdditionalTerms.Trim()}\n";
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
                    PreviousLeaseId = oldLease.LeaseId,  // ✅ Link với lease cũ
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _unitOfWork.Leases.CreateAsync(newLease);

                // Cập nhật lease cũ
                oldLease.Status = "Renewed";
                oldLease.SpecialConditions = string.IsNullOrEmpty(oldLease.SpecialConditions)
                    ? $"Đã gia hạn bằng hợp đồng {newLeaseNumber}"
                    : oldLease.SpecialConditions + $"\n\nĐã gia hạn bằng hợp đồng {newLeaseNumber}";

                await _unitOfWork.Leases.UpdateAsync(oldLease);
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Message = "Gia hạn hợp đồng thành công! Vui lòng ký hợp đồng mới.";
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

    }
}
