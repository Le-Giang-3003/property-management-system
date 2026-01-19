using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    /// <summary>
    /// Interface for user-related operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets all users asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<User>> GetAllUsersAsync();
        /// <summary>
        /// Gets the user by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<User?> GetUserByIdAsync(int id);
        /// <summary>
        /// Gets the users by role asynchronous.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);
        /// <summary>
        /// Registers the specified email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        Task<bool> Register (string email, string username, string password);
    }
}
