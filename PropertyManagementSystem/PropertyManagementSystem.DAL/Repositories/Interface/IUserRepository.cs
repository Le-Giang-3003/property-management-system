using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    /// <summary>
    /// IRepository for user-related operations.
    /// </summary>
    public interface IUserRepository
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
        /// Gets the user by email asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        Task<User?> GetUserByEmailAsync(string email);
        /// <summary>
        /// Adds the new user.
        /// </summary>
        /// <param name="newUser">The new user.</param>
        /// <returns></returns>
        Task<bool> AddNewUser(User newUser);
        Task<User?> GetUserWithRolesByEmailAsync(string email);
        Task<User?> UpdateUserAsync(User user);
    }
}
