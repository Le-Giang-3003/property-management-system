using PropertyManagementSystem.BLL.DTOs.User;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IProfileService
    {
        Task<UserProfileDto?> GetProfileAsync(int userId);
        Task<(bool Success, string Message)> UpdateProfileAsync(int userId, UpdateProfileDto model);
        Task<IEnumerable<UserSearchResultDto>> SearchUsersAsync(UserSearchDto searchDto);
    }
}
