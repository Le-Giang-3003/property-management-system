using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<string>> GetRoleNamesByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role.RoleName)
                .ToListAsync();
        }
    }
}
