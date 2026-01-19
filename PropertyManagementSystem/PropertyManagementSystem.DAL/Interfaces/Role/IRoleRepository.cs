using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Interfaces.Role
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string roleName);
        Task AssignRoleToUserAsync(int userId, int roleId);
    }
}
