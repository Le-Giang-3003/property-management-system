using PropertyManagementSystem.BLL.DTOs.Auth;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IAuthService
    {
        public interface IAuthService
        {
            Task<LoginResult> LoginAsync(LoginRequestDto model);
            Task LogoutAsync();
            Task<UserDto?> GetCurrentUserAsync(int userId);
        }
    }
}
