using PropertyManagementSystem.BLL.DTOs.Admin;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IAdminService
    {
        // Dashboard
        Task<AdminDashboardDto> GetDashboardAsync();

        // User Management
        Task<AdminUserListDto> GetUsersAsync(int pageNumber = 1, int pageSize = 20, string? search = null, string? role = null, string? status = null);
        Task<AdminUserDto?> GetUserByIdAsync(int userId);
        Task<AdminUserDto> CreateUserAsync(CreateUserDto dto, int createdByAdminId);
        Task<bool> UpdateUserAsync(UpdateUserDto dto, int updatedByAdminId);
        Task<bool> ToggleUserStatusAsync(int userId, int adminId);
        Task<bool> ResetUserPasswordAsync(int userId, string newPassword, int adminId);

        // Role Management (only Admin, Member, Technician)
        Task<List<string>> GetAllRolesAsync();
        Task<List<RoleDto>> GetRolesWithUserCountAsync();
        Task<bool> AssignRoleAsync(int userId, string roleName, int adminId);
        Task<bool> RemoveRoleAsync(int userId, string roleName, int adminId);

        // System Configuration
        Task<List<SystemSettingDto>> GetAllSettingsAsync(string? category = null);
        Task<SystemSettingDto?> GetSettingByIdAsync(int settingId);
        Task<SystemSettingDto> CreateSettingAsync(CreateSystemSettingDto dto, int adminId);
        Task<bool> UpdateSettingAsync(UpdateSystemSettingDto dto, int adminId);
        Task<bool> DeleteSettingAsync(int settingId, int adminId);

        // Audit Logs
        Task<AuditLogListDto> GetAuditLogsAsync(int pageNumber = 1, int pageSize = 50, string? search = null, string? action = null, string? entityType = null, DateTime? dateFrom = null, DateTime? dateTo = null);
        Task LogActionAsync(int? userId, string action, string entityType, int? entityId, string? description = null, string? ipAddress = null, string? activeRole = null);
    }
}
