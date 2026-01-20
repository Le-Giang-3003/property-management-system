using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IUserRoleRepository : IGenericRepository<UserRole>
    {
        Task<IEnumerable<string>> GetRoleNamesByUserIdAsync(int userId);
    }
}
