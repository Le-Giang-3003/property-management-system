using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Interfaces.User
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> EmailExistsAsync(string email);
        Task<List<Role>> GetUserRolesAsync(int userId);
    }
}
