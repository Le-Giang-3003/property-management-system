using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.RoleName == roleName);
        }
    }
}
