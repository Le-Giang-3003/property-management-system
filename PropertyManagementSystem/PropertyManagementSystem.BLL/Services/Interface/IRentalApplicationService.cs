using PropertyManagementSystem.BLL.DTOs.Application;
using PropertyManagementSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IRentalApplicationService
    {
        Task<RentalApplication?> GetApplicationByIdAsync(int id);
        Task<IEnumerable<RentalApplication>> GetApplicationsByPropertyAsync(int propertyId);
        Task<IEnumerable<RentalApplication>> GetApplicationsByApplicantAsync(int applicantId);
        Task<IEnumerable<RentalApplication>> GetAllApplicationsAsync();
        Task<RentalApplication?> SubmitApplicationAsync(CreateRentalApplicationDto dto, int applicantId);
        Task<bool> WithdrawApplicationAsync(int applicationId, int userId);
        Task<bool> ApproveApplicationAsync(int applicationId, int reviewerId);
        Task<bool> RejectApplicationAsync(int applicationId, string rejectionReason, int reviewerId);

    }
}
