using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    /// <summary>
    /// Repository for user-related operations.
    /// </summary>
    /// <seealso cref="PropertyManagementSystem.DAL.Repositories.Implementation.GenericRepository&lt;PropertyManagementSystem.DAL.Entities.User&gt;" />
    /// <seealso cref="PropertyManagementSystem.DAL.Repositories.Interface.IUserRepository" />
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets the user by email asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public Task<User?> GetUserByEmailAsync(string email)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        /// <summary>
        /// Gets the users by role asynchronous.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
        {
            return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == roleName)
                     && u.IsActive)
            .ToListAsync();
        }

        /// <summary>
        /// Gets the user with roles by email asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public async Task<User?> GetUserWithRolesByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        /// <summary>
        /// Gets the by phone number asynchronous.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns></returns>
        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }
        /// <summary>
        /// Searchs the users asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="phone">The phone.</param>
        /// <returns></returns>
        public async Task<IEnumerable<User>> SearchUsersAsync(string? name, string? phone)
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(u => u.FullName.ToLower().Contains(name.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                query = query.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(phone));
            }

            return await query.ToListAsync();
        }
        /// <summary>
        /// Determines whether [is phone exists asynchronous] [the specified phone].
        /// </summary>
        /// <param name="phone">The phone.</param>
        /// <param name="excludeUserId">The exclude user identifier.</param>
        /// <returns></returns>
        public async Task<bool> IsPhoneExistsAsync(string phone, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;

            var query = _context.Users.Where(u => u.PhoneNumber == phone);

            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.UserId != excludeUserId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> AddNewUser(User newUser)
        {
            await _context.Users.AddAsync(newUser);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
