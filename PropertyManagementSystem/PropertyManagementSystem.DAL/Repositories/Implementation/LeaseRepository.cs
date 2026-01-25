using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class LeaseRepository : ILeaseRepository
    {
        private readonly AppDbContext _context;

        public LeaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Lease> CreateAsync(Lease lease)
        {
            _context.Leases.Add(lease);
            await _context.SaveChangesAsync();
            return lease;
        }

        public async Task<Lease> GetByIdAsync(int id)
        {
            return await _context.Leases
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .Include(l => l.RentalApplication)
                    .ThenInclude(a => a.Applicant)
                .Include(l => l.PreviousLease)
                .FirstOrDefaultAsync(l => l.LeaseId == id);
        }

        public async Task<IEnumerable<Lease>> GetAllAsync()
        {
            return await _context.Leases
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lease>> GetByPropertyIdAsync(int propertyId)
        {
            return await _context.Leases
                .Include(l => l.Tenant)
                .Where(l => l.PropertyId == propertyId)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lease>> GetByTenantIdAsync(int tenantId)
        {
            return await _context.Leases
                .Include(l => l.Property)
                .Where(l => l.TenantId == tenantId)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lease>> GetByLandlordIdAsync(int landlordId)
        {
            return await _context.Leases
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .Where(l => l.Property.LandlordId == landlordId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<Lease> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.Leases
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .FirstOrDefaultAsync(l => l.ApplicationId == applicationId);
        }

        public async Task<bool> UpdateAsync(Lease lease)
        {
            lease.UpdatedAt = DateTime.UtcNow;
            _context.Leases.Update(lease);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<string> GenerateLeaseNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
            var lastLease = await _context.Leases
                .Where(l => l.LeaseNumber.StartsWith($"LEASE-{year}"))
                .OrderByDescending(l => l.LeaseNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastLease != null)
            {
                var parts = lastLease.LeaseNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int current))
                {
                    nextNumber = current + 1;
                }
            }

            return $"LEASE-{year}-{nextNumber:D4}";
        }

        public async Task<IEnumerable<Lease>> GetLeaseHistoryByPropertyIdAsync(int propertyId)
        {
            return await _context.Leases
                .Include(l => l.Tenant)
                .Where(l => l.PropertyId == propertyId)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<Lease>> GetRenewableLeasesAsync(int daysBeforeExpiry = 30)
        {
            var today = DateTime.UtcNow.Date;
            var expiryDate = today.AddDays(daysBeforeExpiry);

            return await _context.Leases
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .Where(l => l.Status == "Active" &&
                            l.EndDate >= today &&
                            l.EndDate <= expiryDate)
                .OrderBy(l => l.EndDate)
                .ToListAsync();
        }

        public async Task<Lease?> GetLeaseWithDetailsAsync(int leaseId)
        {
            return await _context.Leases
                .Include(l => l.Property)
                    .ThenInclude(p => p.Landlord)
                .Include(l => l.Tenant)
                .Include(l => l.RentalApplication)
                .Include(l => l.Signatures)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(l => l.LeaseId == leaseId);
        }

        public async Task<IEnumerable<Lease>> GetLeasesByStatusAsync(string status)
        {
            return await _context.Leases
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .Where(l => l.Status == status)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lease>> GetLeasesByTenantUserIdAsync(int tenantUserId)
        {
            return await _dbSet
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .Where(l => l.Tenant.UserId == tenantUserId)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }

        public async Task<List<Lease>> GetExpiringByLandlordAsync(int landlordId, int daysAhead = 30)
        {
            var today = DateTime.UtcNow;
            var futureDate = DateTime.UtcNow.AddDays(daysAhead);

            return await _dbSet
                .Include(c => c.Property)
                .Include(c => c.Tenant)
                .Where(c => c.Property.LandlordId == landlordId &&
                           c.Status == "Active" &&
                           c.EndDate >= today &&
                           c.EndDate <= futureDate)
                .OrderBy(c => c.EndDate)
                .ToListAsync();
        }
    }
}
