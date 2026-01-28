using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Implementation;
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
        /// The otp service
        /// </summary>
        private readonly IStatelessOtpService _otpService;
        private readonly IGenericRepository<User> _userGenericRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService" /> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="passwordService">The password service.</param>
        /// <param name="otpService">The otp service.</param>
        public UserService(IUserRepository userRepository, IPasswordService passwordService, IStatelessOtpService otpService, IGenericRepository<User> userGenericRepository)
        {
            _repo = userRepository;
            _passwordService = passwordService;
            _otpService = otpService;
            _userGenericRepository = userGenericRepository;
        }
        /// <summary>
        /// Gets all users asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userGenericRepository.GetAllAsync();
        }

        /// <summary>
        /// Gets the user by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userGenericRepository.GetByIdAsync(id);
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
        /// Registers the with verified email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public async Task<bool> RegisterWithVerifiedEmail(string email, string username, string password)
        {
            var passwordHash = _passwordService.HashPassword(password);
            var newUser = new User
            {
                Email = email,
                PasswordHash = passwordHash,
                FullName = username, // Temporary assignment, can be modified later
                IsActive = true,
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        RoleId = 2, // cho mặc định là Member
                        AssignedAt = DateTime.UtcNow
                    }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            return await _repo.AddNewUser(newUser);
        }

        /// <summary>
        /// Sends the registration otp asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Email already exists.</exception>
        public async Task<string> SendRegistrationOtpAsync(string email)
        {
            // Check if email already exists
            var existingUser = await _repo.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            // Generate and send OTP
            var otpHash = await _otpService.GenerateOtpHashAsync(email);
            return otpHash;
        }

        /// <summary>
        /// Verifies the registration otp.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="otp">The otp.</param>
        /// <param name="otpHash">The otp hash.</param>
        /// <returns></returns>
        public (bool IsValid, string Message) VerifyRegistrationOtp(string email, string otp, string otpHash)
        {
            return _otpService.VerifyOtpAsync(email, otp, otpHash);
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _repo.GetUserByEmailAsync(email);
        }
    }
}
