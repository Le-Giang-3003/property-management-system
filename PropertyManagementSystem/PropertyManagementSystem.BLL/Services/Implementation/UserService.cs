using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    /// <summary>
    /// Service for user-related operations.
    /// </summary>
    /// <seealso cref="PropertyManagementSystem.BLL.Services.Interface.IUserService" />
    public class UserService : IUserService
    {
        /// <summary>
        /// The repo
        /// </summary>
        private readonly IUserRepository _repo;
        /// <summary>
        /// The password service
        /// </summary>
        private readonly IPasswordService _passwordService;
        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="passwordService">The password service.</param>
        public UserService(IUserRepository userRepository, IPasswordService passwordService)
        {
            _repo = userRepository;
            _passwordService = passwordService;
        }
        /// <summary>
        /// Gets all users asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _repo.GetAllUsersAsync();
        }

        /// <summary>
        /// Gets the user by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _repo.GetUserByIdAsync(id);
        }

        /// <summary>
        /// Gets the users by role asynchronous.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
        {
            return await _repo.GetUsersByRoleAsync(roleName);
        }

        /// <summary>
        /// Registers the specified email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public async Task<bool> Register(string email, string username,string password)
        {
            var existingUser = await _repo.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                return false; // User with the same email already exists
            }

            var passwordHash = _passwordService.HashPassword(password);

            var newUser = new User
            {
                Email = email,
                PasswordHash = passwordHash,
                FullName = username, // Temporary assignment, can be modified later
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var created = await _repo.AddNewUser(newUser);
            if(created)
            {
                return true;
            }   
            return false;
        }
    }
}
