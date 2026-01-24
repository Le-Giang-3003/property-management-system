using PropertyManagementSystem.BLL.DTOs.Application;
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
    public class RentalApplicationService : IRentalApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RentalApplicationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RentalApplication?> GetApplicationByIdAsync(int id)
        {
            return await _unitOfWork.RentalApplications.GetByIdAsync(id);
        }

        public async Task<IEnumerable<RentalApplication>> GetApplicationsByPropertyAsync(int propertyId)
        {
            return await _unitOfWork.RentalApplications.GetByPropertyIdAsync(propertyId);
        }

        public async Task<IEnumerable<RentalApplication>> GetApplicationsByApplicantAsync(int applicantId)
        {
            return await _unitOfWork.RentalApplications.GetByApplicantIdAsync(applicantId);
        }

        public async Task<IEnumerable<RentalApplication>> GetAllApplicationsAsync()
        {
            return await _unitOfWork.RentalApplications.GetAllAsync();
        }

        public async Task<RentalApplication?> SubmitApplicationAsync(CreateRentalApplicationDto dto, int applicantId)
        {
            var property = await _unitOfWork.Properties.GetPropertyByIdAsync(dto.PropertyId);
            if (property == null || property.Status != "Available")
            {
                return null;
            }

            var applicationNumber = await _unitOfWork.RentalApplications.GenerateApplicationNumberAsync();

            var application = new RentalApplication
            {
                PropertyId = dto.PropertyId,
                ApplicantId = applicantId,
                ApplicationNumber = applicationNumber,
                EmploymentStatus = dto.EmploymentStatus,
                Employer = dto.Employer,
                MonthlyIncome = dto.MonthlyIncome,
                PreviousAddress = dto.PreviousAddress,
                PreviousLandlord = dto.PreviousLandlord,
                PreviousLandlordPhone = dto.PreviousLandlordPhone,
                NumberOfOccupants = dto.NumberOfOccupants,
                HasPets = dto.HasPets,
                PetDetails = dto.PetDetails,
                DesiredMoveInDate = dto.DesiredMoveInDate,
                AdditionalNotes = dto.AdditionalNotes,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var result = await _unitOfWork.RentalApplications.CreateAsync(application);
            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        public async Task<bool> WithdrawApplicationAsync(int applicationId, int userId)
        {
            var application = await _unitOfWork.RentalApplications.GetByIdAsync(applicationId);

            if (application == null || application.ApplicantId != userId)
                return false;

            if (application.Status == "Approved" || application.Status == "Rejected")
                return false;

            application.Status = "Withdrawn";
            application.UpdatedAt = DateTime.UtcNow;

            var updated = await _unitOfWork.RentalApplications.UpdateAsync(application);
            if (updated)
            {
                await _unitOfWork.SaveChangesAsync();
            }

            return updated;
        }

        // ✅ APPROVE - UPDATE PROPERTY STATUS → "RENTED"
        public async Task<bool> ApproveApplicationAsync(int applicationId, int reviewerId)
        {
            // 1. Lấy Application
            var application = await _unitOfWork.RentalApplications.GetByIdAsync(applicationId);

            if (application == null)
                return false;

            if (application.Status == "Withdrawn")
                return false;

            // 2. ✅ Lấy Property
            var property = await _unitOfWork.Properties.GetPropertyByIdAsync(application.PropertyId);
            if (property == null)
                return false;

            // 3. ✅ Update Application
            application.Status = "Approved";
            application.ReviewedBy = reviewerId;
            application.ReviewedAt = DateTime.UtcNow;
            application.UpdatedAt = DateTime.UtcNow;
            application.RejectionReason = null;

            var appUpdated = await _unitOfWork.RentalApplications.UpdateAsync(application);
            if (!appUpdated)
                return false;

            // 4. ✅ Update Property → Status = "Rented"
            property.Status = "Rented";
            property.UpdatedAt = DateTime.UtcNow;

            var propertyUpdated = await _unitOfWork.Properties.UpdatePropertyAsync(property);
            if (!propertyUpdated)
                return false;

            // 5. ✅ Tự động REJECT tất cả đơn khác của cùng Property
            var otherApplications = await _unitOfWork.RentalApplications.GetByPropertyIdAsync(application.PropertyId);

            foreach (var otherApp in otherApplications)
            {
                if (otherApp.ApplicationId != applicationId &&
                    (otherApp.Status == "Pending" || otherApp.Status == "UnderReview"))
                {
                    otherApp.Status = "Rejected";
                    otherApp.RejectionReason = "Bất động sản đã được cho thuê cho người khác";
                    otherApp.ReviewedBy = reviewerId;
                    otherApp.ReviewedAt = DateTime.UtcNow;
                    otherApp.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.RentalApplications.UpdateAsync(otherApp);
                }
            }

            // 6. ✅ Save tất cả thay đổi
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectApplicationAsync(int applicationId, string rejectionReason, int reviewerId)
        {
            var application = await _unitOfWork.RentalApplications.GetByIdAsync(applicationId);

            if (application == null)
                return false;

            if (application.Status == "Withdrawn")
                return false;

            if (string.IsNullOrWhiteSpace(rejectionReason))
                return false;

            application.Status = "Rejected";
            application.RejectionReason = rejectionReason;
            application.ReviewedBy = reviewerId;
            application.ReviewedAt = DateTime.UtcNow;
            application.UpdatedAt = DateTime.UtcNow;

            var updated = await _unitOfWork.RentalApplications.UpdateAsync(application);
            if (updated)
            {
                await _unitOfWork.SaveChangesAsync();
            }

            return updated;
        }
    }
}
