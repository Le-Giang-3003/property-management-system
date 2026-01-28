namespace PropertyManagementSystem.BLL.Services.Interface
{
    /// <summary>
    /// Interface for password service.
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Hashes the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        string HashPassword(string password);
        /// <summary>
        /// Verifies the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="passwordHash">The password hash.</param>
        /// <returns></returns>
        bool VerifyPassword(string password, string passwordHash);
    }
}
