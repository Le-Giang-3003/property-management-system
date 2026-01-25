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
    public class LeaseSignatureRepository : ILeaseSignatureRepository
    {
        private readonly AppDbContext _context;

        public LeaseSignatureRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LeaseSignature> CreateAsync(LeaseSignature signature)
        {
            await _context.LeaseSignatures.AddAsync(signature);
            return signature;
        }

        public async Task<IEnumerable<LeaseSignature>> GetByLeaseIdAsync(int leaseId)
        {
            return await _context.LeaseSignatures
                .Include(s => s.User)
                .Where(s => s.LeaseId == leaseId)
                .OrderBy(s => s.SignedAt)
                .ToListAsync();
        }

        public async Task<LeaseSignature?> GetByLeaseAndUserAsync(int leaseId, int userId)
        {
            return await _context.LeaseSignatures
                .FirstOrDefaultAsync(s => s.LeaseId == leaseId && s.UserId == userId);
        }

        public async Task<bool> HasLandlordSignedAsync(int leaseId)
        {
            return await _context.LeaseSignatures
                .AnyAsync(s => s.LeaseId == leaseId && s.SignerRole == "Landlord");
        }

        public async Task<bool> HasTenantSignedAsync(int leaseId)
        {
            return await _context.LeaseSignatures
                .AnyAsync(s => s.LeaseId == leaseId && s.SignerRole == "Tenant");
        }

        public async Task<bool> IsFullySignedAsync(int leaseId)
        {
            var signatures = await _context.LeaseSignatures
                .Where(s => s.LeaseId == leaseId)
                .Select(s => s.SignerRole)
                .ToListAsync();

            return signatures.Contains("Landlord") && signatures.Contains("Tenant");
        }
    }
}
