using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    /// <summary>
    /// IRepository for user-related operations.
    /// </summary>
    /// <seealso cref="PropertyManagementSystem.DAL.Repositories.Interface.IGenericRepository&lt;PropertyManagementSystem.DAL.Entities.User&gt;" />
    /// <seealso cref="PropertyManagementSystem.DAL.Repositories.Interface.IGenericRepository&lt;PropertyManagementSystem.DAL.Entities.User&gt;" />
    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// Adds the new user.
        /// </summary>
        /// <param name="newUser">The new user.</param>
        /// <returns></returns>
        Task<bool> AddNewUser(User newUser);
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
        /// Gets the user with roles by email asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        Task<User?> GetUserWithRolesByEmailAsync(string email);
        /// <summary>
        /// Gets the by phone number asynchronous.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns></returns>
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
        /// <summary>
        /// Searchs the users asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="phone">The phone.</param>
        /// <returns></returns>
        Task<IEnumerable<User>> SearchUsersAsync(string? name, string? phone);
        /// <summary>
        /// Determines whether [is phone exists asynchronous] [the specified phone].
        /// </summary>
        /// <param name="phone">The phone.</param>
        /// <param name="excludeUserId">The exclude user identifier.</param>
        /// <returns></returns>
        Task<bool> IsPhoneExistsAsync(string phone, int? excludeUserId = null);
        Task<User?> UpdateUserAsync(User user);
    }
}
