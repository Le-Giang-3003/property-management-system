using PropertyManagementSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IRentalApplicationRepository
    {
        Task<RentalApplication> CreateAsync(RentalApplication application);
        Task<RentalApplication> GetByIdAsync(int id);
        Task<IEnumerable<RentalApplication>> GetByApplicantIdAsync(int applicantId);
        Task<IEnumerable<RentalApplication>> GetByPropertyIdAsync(int propertyId);
        Task<IEnumerable<RentalApplication>> GetAllAsync();
        Task<IEnumerable<RentalApplication>> GetByStatusAsync(string status);
        Task<bool> UpdateAsync(RentalApplication application);
        Task<string> GenerateApplicationNumberAsync();
    }
}
