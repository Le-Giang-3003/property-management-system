using PropertyManagementSystem.BLL.Services.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    /// <summary>
    /// Password service.
    /// </summary>
    /// <seealso cref="PropertyManagementSystem.BLL.Services.Interface.IPasswordService" />
    public class PasswordService : IPasswordService
    {
        /// <summary>
        /// Hashes the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="passwordHash">The password hash.</param>
        /// <returns></returns>
        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
