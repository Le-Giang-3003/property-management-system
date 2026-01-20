namespace PropertyManagementSystem.BLL.Services.Interface
{
    /// <summary>
    /// Interface for stateless OTP service.
    /// </summary>
    public interface IStatelessOtpService
    {
        /// <summary>
        /// Generates the otp hash.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        Task<string> GenerateOtpHashAsync(string email);
        /// <summary>
        /// Verifies the otp.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="userOtp">The user otp.</param>
        /// <param name="otpHash">The otp hash.</param>
        /// <returns></returns>
        bool VerifyOtpAsync(string email, string userOtp, string receivedHash);
    }
}
