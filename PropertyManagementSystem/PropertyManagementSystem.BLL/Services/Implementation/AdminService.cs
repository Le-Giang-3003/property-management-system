using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.BLL.DTOs.Admin;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;
        private readonly IGenericRepository<AuditLog> _auditLogRepo;
        private readonly IGenericRepository<SystemSetting> _systemSettingRepo;
        private readonly IGenericRepository<Property> _propertyRepo;
        private readonly IGenericRepository<Lease> _leaseRepo;
        private readonly IGenericRepository<MaintenanceRequest> _maintenanceRepo;

        public AdminService(
            IUnitOfWork unitOfWork,
            IPasswordService passwordService,
            IGenericRepository<AuditLog> auditLogRepo,
            IGenericRepository<SystemSetting> systemSettingRepo,
            IGenericRepository<Property> propertyRepo,
            IGenericRepository<Lease> leaseRepo,
            IGenericRepository<MaintenanceRequest> maintenanceRepo)
        {
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
            _auditLogRepo = auditLogRepo;
            _systemSettingRepo = systemSettingRepo;
            _propertyRepo = propertyRepo;
            _leaseRepo = leaseRepo;
            _maintenanceRepo = maintenanceRepo;
        }

        #region Dashboard

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            var allUsers = await _unitOfWork.Users.GetAllAsync();
            var userList = allUsers.ToList();

            var allUserRoles = await _unitOfWork.UserRoles.QueryWithIncludes(ur => ur.Role).ToListAsync();

            var totalProperties = await _propertyRepo.CountAsync();
            var activeLeases = await _leaseRepo.CountAsync(l => l.Status == "Active");
            var pendingMaintenance = await _maintenanceRepo.CountAsync(m => m.Status == "Pending");

            var recentLogs = await _auditLogRepo.Query()
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToListAsync();

            return new AdminDashboardDto
            {
                TotalUsers = userList.Count,
                ActiveUsers = userList.Count(u => u.IsActive),
                TotalMembers = allUserRoles.Count(ur => ur.Role.RoleName == "Member"),
                TotalTechnicians = allUserRoles.Count(ur => ur.Role.RoleName == "Technician"),
                TotalAdmins = allUserRoles.Count(ur => ur.Role.RoleName == "Admin"),
                TotalProperties = totalProperties,
                ActiveLeases = activeLeases,
                PendingMaintenanceRequests = pendingMaintenance,
                RecentAuditLogs = await _auditLogRepo.CountAsync(a => a.CreatedAt >= DateTime.UtcNow.AddDays(-7)),
                LatestAuditLogs = recentLogs.Select(MapToAuditLogDto).ToList()
            };
        }

        #endregion

        #region User Management

        public async Task<AdminUserListDto> GetUsersAsync(int pageNumber = 1, int pageSize = 20, string? search = null, string? role = null, string? status = null)
        {
            var query = _unitOfWork.Users.Query()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(term) || u.Email.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == role));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "Active")
                    query = query.Where(u => u.IsActive);
                else if (status == "Inactive")
                    query = query.Where(u => !u.IsActive);
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new AdminUserListDto
            {
                Users = users.Select(MapToAdminUserDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = search,
                RoleFilter = role,
                StatusFilter = status
            };
        }

        public async Task<AdminUserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _unitOfWork.Users.Query()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            return user == null ? null : MapToAdminUserDto(user);
        }

        public async Task<AdminUserDto> CreateUserAsync(CreateUserDto dto, int createdByAdminId)
        {
            if (!AllowedRoles.Contains(dto.Role))
                throw new InvalidOperationException("Role must be one of: Admin, Member, Technician.");

            var existingUser = await _unitOfWork.Users.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email already exists.");

            var role = await _unitOfWork.Roles.GetByNameAsync(dto.Role);
            if (role == null)
                throw new InvalidOperationException($"Role '{dto.Role}' not found.");

            var passwordHash = _passwordService.HashPassword(dto.Password);

            var newUser = new User
            {
                Email = dto.Email,
                FullName = dto.FullName,
                PasswordHash = passwordHash,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                IsActive = true,
                EmailVerified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            var userRole = new UserRole
            {
                UserId = newUser.UserId,
                RoleId = role.RoleId,
                AssignedAt = DateTime.UtcNow
            };
            await _unitOfWork.UserRoles.AddAsync(userRole);
            await _unitOfWork.SaveChangesAsync();

            await LogActionAsync(createdByAdminId, "Create", "User", newUser.UserId,
                $"Created user '{newUser.Email}' with role '{dto.Role}'");

            return new AdminUserDto
            {
                UserId = newUser.UserId,
                Email = newUser.Email,
                FullName = newUser.FullName,
                PhoneNumber = newUser.PhoneNumber,
                Address = newUser.Address,
                IsActive = newUser.IsActive,
                EmailVerified = newUser.EmailVerified,
                CreatedAt = newUser.CreatedAt,
                Roles = new List<string> { dto.Role }
            };
        }

        public async Task<bool> UpdateUserAsync(UpdateUserDto dto, int updatedByAdminId)
        {
            var user = await _unitOfWork.Users.Query()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId);

            if (user == null) return false;

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateUserAsync(user);

            if (!string.IsNullOrWhiteSpace(dto.Role) && AllowedRoles.Contains(dto.Role))
            {
                var currentRole = user.UserRoles.FirstOrDefault()?.Role?.RoleName;
                if (currentRole != dto.Role)
                {
                    var newRole = await _unitOfWork.Roles.GetByNameAsync(dto.Role);
                    if (newRole != null)
                    {
                        var existingUserRoles = user.UserRoles.ToList();
                        foreach (var ur in existingUserRoles)
                        {
                            _unitOfWork.UserRoles.Delete(ur);
                        }
                        await _unitOfWork.SaveChangesAsync();

                        var newUserRole = new UserRole
                        {
                            UserId = user.UserId,
                            RoleId = newRole.RoleId,
                            AssignedAt = DateTime.UtcNow,
                            AssignedBy = updatedByAdminId
                        };
                        await _unitOfWork.UserRoles.AddAsync(newUserRole);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
            }

            await LogActionAsync(updatedByAdminId, "Update", "User", user.UserId,
                $"Updated user '{user.Email}'");

            return true;
        }

        public async Task<bool> ToggleUserStatusAsync(int userId, int adminId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return false;

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateUserAsync(user);

            await LogActionAsync(adminId, user.IsActive ? "Activate" : "Deactivate", "User", userId,
                $"{(user.IsActive ? "Activated" : "Deactivated")} user '{user.Email}'");

            return true;
        }

        public async Task<bool> ResetUserPasswordAsync(int userId, string newPassword, int adminId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return false;

            user.PasswordHash = _passwordService.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateUserAsync(user);

            await LogActionAsync(adminId, "ResetPassword", "User", userId,
                $"Reset password for user '{user.Email}'");

            return true;
        }

        #endregion

        private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase) { "Admin", "Member", "Technician" };

        #region Role Management

        public async Task<List<string>> GetAllRolesAsync()
        {
            var roles = await _unitOfWork.Roles.GetAllAsync();
            return roles.Where(r => AllowedRoles.Contains(r.RoleName)).Select(r => r.RoleName).ToList();
        }

        public async Task<List<RoleDto>> GetRolesWithUserCountAsync()
        {
            var roles = await _unitOfWork.Roles.QueryWithIncludes(r => r.UserRoles).ToListAsync();
            var result = new List<RoleDto>();
            foreach (var r in roles.Where(r => AllowedRoles.Contains(r.RoleName)))
            {
                result.Add(new RoleDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    Description = r.Description,
                    UserCount = r.UserRoles?.Count ?? 0
                });
            }
            return result;
        }

        public async Task<bool> AssignRoleAsync(int userId, string roleName, int adminId)
        {
            var user = await _unitOfWork.Users.Query()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return false;

            if (user.UserRoles.Any(ur => ur.Role.RoleName == roleName))
                return true;

            var role = await _unitOfWork.Roles.GetByNameAsync(roleName);
            if (role == null) return false;

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = role.RoleId,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = adminId
            };

            await _unitOfWork.UserRoles.AddAsync(userRole);
            await _unitOfWork.SaveChangesAsync();

            await LogActionAsync(adminId, "AssignRole", "UserRole", userId,
                $"Assigned role '{roleName}' to user '{user.Email}'");

            return true;
        }

        public async Task<bool> RemoveRoleAsync(int userId, string roleName, int adminId)
        {
            var user = await _unitOfWork.Users.Query()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return false;

            var userRole = user.UserRoles.FirstOrDefault(ur => ur.Role.RoleName == roleName);
            if (userRole == null) return false;

            if (user.UserRoles.Count <= 1)
                throw new InvalidOperationException("Cannot remove the last role from a user.");

            _unitOfWork.UserRoles.Delete(userRole);
            await _unitOfWork.SaveChangesAsync();

            await LogActionAsync(adminId, "RemoveRole", "UserRole", userId,
                $"Removed role '{roleName}' from user '{user.Email}'");

            return true;
        }

        #endregion

        #region System Configuration

        public async Task<List<SystemSettingDto>> GetAllSettingsAsync(string? category = null)
        {
            var query = _systemSettingRepo.Query()
                .Include(s => s.UpdatedByUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(s => s.Category == category);
            }

            var settings = await query.OrderBy(s => s.Category).ThenBy(s => s.SettingKey).ToListAsync();

            return settings.Select(s => new SystemSettingDto
            {
                SettingId = s.SettingId,
                SettingKey = s.SettingKey,
                SettingValue = s.SettingValue,
                Category = s.Category,
                Description = s.Description,
                DataType = s.DataType,
                IsPublic = s.IsPublic,
                UpdatedAt = s.UpdatedAt,
                UpdatedByName = s.UpdatedByUser?.FullName
            }).ToList();
        }

        public async Task<SystemSettingDto?> GetSettingByIdAsync(int settingId)
        {
            var setting = await _systemSettingRepo.Query()
                .Include(s => s.UpdatedByUser)
                .FirstOrDefaultAsync(s => s.SettingId == settingId);

            if (setting == null) return null;

            return new SystemSettingDto
            {
                SettingId = setting.SettingId,
                SettingKey = setting.SettingKey,
                SettingValue = setting.SettingValue,
                Category = setting.Category,
                Description = setting.Description,
                DataType = setting.DataType,
                IsPublic = setting.IsPublic,
                UpdatedAt = setting.UpdatedAt,
                UpdatedByName = setting.UpdatedByUser?.FullName
            };
        }

        public async Task<SystemSettingDto> CreateSettingAsync(CreateSystemSettingDto dto, int adminId)
        {
            var existing = await _systemSettingRepo.FirstOrDefaultAsync(s => s.SettingKey == dto.SettingKey);
            if (existing != null)
                throw new InvalidOperationException($"Setting key '{dto.SettingKey}' already exists.");

            var setting = new SystemSetting
            {
                SettingKey = dto.SettingKey,
                SettingValue = dto.SettingValue,
                Category = dto.Category,
                Description = dto.Description,
                DataType = dto.DataType,
                IsPublic = dto.IsPublic,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = adminId
            };

            await _systemSettingRepo.AddAsync(setting);
            await _unitOfWork.SaveChangesAsync();

            await LogActionAsync(adminId, "Create", "SystemSetting", setting.SettingId,
                $"Created setting '{dto.SettingKey}'");

            return new SystemSettingDto
            {
                SettingId = setting.SettingId,
                SettingKey = setting.SettingKey,
                SettingValue = setting.SettingValue,
                Category = setting.Category,
                Description = setting.Description,
                DataType = setting.DataType,
                IsPublic = setting.IsPublic,
                UpdatedAt = setting.UpdatedAt
            };
        }

        public async Task<bool> UpdateSettingAsync(UpdateSystemSettingDto dto, int adminId)
        {
            var setting = await _systemSettingRepo.GetByIdAsync(dto.SettingId);
            if (setting == null) return false;

            setting.SettingValue = dto.SettingValue;
            if (dto.Description != null)
                setting.Description = dto.Description;
            setting.UpdatedAt = DateTime.UtcNow;
            setting.UpdatedBy = adminId;

            _systemSettingRepo.Update(setting);
            await _unitOfWork.SaveChangesAsync();

            await LogActionAsync(adminId, "Update", "SystemSetting", setting.SettingId,
                $"Updated setting '{setting.SettingKey}'");

            return true;
        }

        public async Task<bool> DeleteSettingAsync(int settingId, int adminId)
        {
            var setting = await _systemSettingRepo.GetByIdAsync(settingId);
            if (setting == null) return false;

            var key = setting.SettingKey;
            _systemSettingRepo.Delete(setting);
            await _unitOfWork.SaveChangesAsync();

            await LogActionAsync(adminId, "Delete", "SystemSetting", settingId,
                $"Deleted setting '{key}'");

            return true;
        }

        #endregion

        #region Audit Logs

        public async Task<AuditLogListDto> GetAuditLogsAsync(int pageNumber = 1, int pageSize = 50, string? search = null, string? action = null, string? entityType = null, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            var query = _auditLogRepo.Query()
                .Include(a => a.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(a =>
                    (a.Description != null && a.Description.ToLower().Contains(term)) ||
                    (a.User != null && a.User.Email.ToLower().Contains(term)) ||
                    (a.User != null && a.User.FullName.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(a => a.Action == action);

            if (!string.IsNullOrWhiteSpace(entityType))
                query = query.Where(a => a.EntityType == entityType);

            if (dateFrom.HasValue)
                query = query.Where(a => a.CreatedAt >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(a => a.CreatedAt <= dateTo.Value.AddDays(1));

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new AuditLogListDto
            {
                Logs = logs.Select(MapToAuditLogDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = search,
                ActionFilter = action,
                EntityTypeFilter = entityType,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
        }

        public async Task LogActionAsync(int? userId, string action, string entityType, int? entityId, string? description = null, string? ipAddress = null, string? activeRole = null)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action ?? "",
                EntityType = entityType ?? "",
                EntityId = entityId,
                Description = description ?? "",
                IpAddress = ipAddress ?? "",
                UserAgent = "",
                OldValues = "",
                NewValues = "",
                ActiveRole = activeRole ?? "Admin",
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _auditLogRepo.AddAsync(log);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                // Audit is best-effort; do not fail the main operation (e.g. CreateUser)
            }
        }

        #endregion

        #region Private Helpers

        private AdminUserDto MapToAdminUserDto(User user)
        {
            return new AdminUserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Avatar = user.Avatar,
                DateOfBirth = user.DateOfBirth,
                IsActive = user.IsActive,
                EmailVerified = user.EmailVerified,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Roles = user.UserRoles?.Select(ur => ur.Role?.RoleName ?? "").Where(r => !string.IsNullOrEmpty(r)).ToList() ?? new List<string>()
            };
        }

        private AuditLogDto MapToAuditLogDto(AuditLog log)
        {
            return new AuditLogDto
            {
                LogId = log.LogId,
                UserId = log.UserId,
                UserName = log.User?.FullName,
                UserEmail = log.User?.Email,
                Action = log.Action,
                EntityType = log.EntityType,
                EntityId = log.EntityId,
                IpAddress = log.IpAddress,
                Description = log.Description,
                ActiveRole = log.ActiveRole,
                CreatedAt = log.CreatedAt
            };
        }

        #endregion
    }

}
