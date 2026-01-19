using PropertyManagementSystem.BLL.DTOs.Auth;

namespace PropertyManagementSystem.BLL.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}
