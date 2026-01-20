using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetUserWithRolesAsync(int userId);
        Task<User?> GetUserWithRolesByEmailAsync(string email);
    }
}
