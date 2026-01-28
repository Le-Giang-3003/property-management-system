using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class RentalApplicationRepository : IRentalApplicationRepository
    {
        private readonly AppDbContext _context;

        public RentalApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RentalApplication> CreateAsync(RentalApplication application)
        {
            _context.RentalApplications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<RentalApplication> GetByIdAsync(int id)
        {
            return await _context.RentalApplications
                .Include(r => r.Property)
                .Include(r => r.Applicant)
                .Include(r => r.ReviewedByUser)
                .Include(r => r.Lease)
                .FirstOrDefaultAsync(r => r.ApplicationId == id);
        }

        public async Task<IEnumerable<RentalApplication>> GetByApplicantIdAsync(int applicantId)
        {
            return await _context.RentalApplications
                .Include(r => r.Property)
                .Include(r => r.Lease)
                .Where(r => r.ApplicantId == applicantId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<RentalApplication>> GetByPropertyIdAsync(int propertyId)
        {
            return await _context.RentalApplications
                .Include(r => r.Applicant)
                .Include(r => r.Lease)
                .Where(r => r.PropertyId == propertyId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<RentalApplication>> GetAllAsync()
        {
            return await _context.RentalApplications
                .Include(r => r.Property)
                .Include(r => r.Applicant)
                .Include(r => r.ReviewedByUser)
                .Include(r => r.Lease)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<RentalApplication>> GetByStatusAsync(string status)
        {
            return await _context.RentalApplications
                .Include(r => r.Property)
                .Include(r => r.Applicant)
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(RentalApplication application)
        {
            application.UpdatedAt = DateTime.UtcNow;
            _context.RentalApplications.Update(application);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<string> GenerateApplicationNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
            var lastApp = await _context.RentalApplications
                .Where(r => r.ApplicationNumber.StartsWith($"APP-{year}"))
                .OrderByDescending(r => r.ApplicationNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastApp != null)
            {
                var parts = lastApp.ApplicationNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int current))
                {
                    nextNumber = current + 1;
                }
            }

            return $"APP-{year}-{nextNumber:D4}";
        }

        public async Task<List<RentalApplication>> GetRecentByLandlordAsync(int landlordId, int take = 5)
        {
            return await _context.RentalApplications
                .Include(a => a.Property)
                .Include(a => a.Applicant)
                .Where(a => a.Property.LandlordId == landlordId)
                .OrderByDescending(a => a.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetPendingCountByLandlordAsync(int landlordId)
        {
            return await _context.RentalApplications
                .Include(a => a.Property)
                .CountAsync(a => a.Property.LandlordId == landlordId && a.Status == "Pending");
        }
    }
}
