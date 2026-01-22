using PropertyManagementSystem.BLL.DTOs.Auth;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginRequestDto model);
        Task LogoutAsync();
        Task<UserDto?> GetCurrentUserAsync(int userId);

        Task<bool> SendOtpEmailAsync(ForgotPasswordRequestDTO request);
        Task<(bool isValid, int userId)> VerifyOtpAsync(VerifyOtpRequestDTO request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDTO request);
        Task<bool> ChangePasswordAsync(string email, ChangePasswordRequestDTO request);
    }
}
