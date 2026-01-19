using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Interfaces.Role;

namespace PropertyManagementSystem.DAL.Repositories.Role
{
    public class RoleRepository : IRoleRepository
    {
        private readonly PropertyManagementDbContext _context;

        public RoleRepository(PropertyManagementDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task AssignRoleToUserAsync(int userId, int roleId)
        {
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        }
    }
}
