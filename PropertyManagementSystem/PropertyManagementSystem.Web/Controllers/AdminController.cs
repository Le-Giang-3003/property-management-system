using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Admin;
using PropertyManagementSystem.BLL.Services.Interface;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace PropertyManagementSystem.Web.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        #region Dashboard

        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboard = await _adminService.GetDashboardAsync();
                return View(dashboard);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading dashboard: {ex.Message}";
                return View(new AdminDashboardDto());
            }
        }

        #endregion

        #region User Management

        public async Task<IActionResult> Users(int page = 1, string? search = null, string? role = null, string? status = null)
        {
            try
            {
                var result = await _adminService.GetUsersAsync(page, 20, search, role, status);
                ViewBag.Roles = await _adminService.GetAllRolesAsync();
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading users: {ex.Message}";
                return View(new AdminUserListDto());
            }
        }

        public async Task<IActionResult> UserDetail(int id)
        {
            try
            {
                var user = await _adminService.GetUserByIdAsync(id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction(nameof(Users));
                }

                ViewBag.Roles = await _adminService.GetAllRolesAsync();
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading user: {ex.Message}";
                return RedirectToAction(nameof(Users));
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            ViewBag.Roles = await _adminService.GetAllRolesAsync();
            return View(new CreateUserDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _adminService.GetAllRolesAsync();
                return View(dto);
            }

            try
            {
                var adminId = GetCurrentUserId();
                await _adminService.CreateUserAsync(dto, adminId);
                TempData["SuccessMessage"] = $"User '{dto.Email}' created successfully!";
                return RedirectToAction(nameof(Users));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Roles = await _adminService.GetAllRolesAsync();
                return View(dto);
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                TempData["ErrorMessage"] = $"Error creating user: {inner}";
                ViewBag.Roles = await _adminService.GetAllRolesAsync();
                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating user: {ex.Message}";
                ViewBag.Roles = await _adminService.GetAllRolesAsync();
                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data.";
                return RedirectToAction(nameof(UserDetail), new { id = dto.UserId });
            }

            try
            {
                var adminId = GetCurrentUserId();
                var result = await _adminService.UpdateUserAsync(dto, adminId);
                TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
                    ? "User updated successfully!"
                    : "User not found.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating user: {ex.Message}";
            }

            return RedirectToAction(nameof(UserDetail), new { id = dto.UserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(int userId)
        {
            try
            {
                var adminId = GetCurrentUserId();
                var result = await _adminService.ToggleUserStatusAsync(userId, adminId);
                TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
                    ? "User status updated!"
                    : "User not found.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error toggling user status: {ex.Message}";
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                TempData["ErrorMessage"] = "Password must be at least 6 characters.";
                return RedirectToAction(nameof(UserDetail), new { id = userId });
            }

            try
            {
                var adminId = GetCurrentUserId();
                var result = await _adminService.ResetUserPasswordAsync(userId, newPassword, adminId);
                TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
                    ? "Password reset successfully!"
                    : "User not found.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error resetting password: {ex.Message}";
            }

            return RedirectToAction(nameof(UserDetail), new { id = userId });
        }

        [HttpGet]
        public async Task<IActionResult> Roles()
        {
            try
            {
                var roles = await _adminService.GetRolesWithUserCountAsync();
                return View(roles);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading roles: {ex.Message}";
                return View(new List<RoleDto>());
            }
        }

        #endregion

        #region System Configuration

        public async Task<IActionResult> SystemSettings(string? category = null)
        {
            try
            {
                var settings = await _adminService.GetAllSettingsAsync(category);
                ViewBag.SelectedCategory = category;
                return View(settings);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading settings: {ex.Message}";
                return View(new List<SystemSettingDto>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSetting(CreateSystemSettingDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid setting data.";
                return RedirectToAction(nameof(SystemSettings));
            }

            try
            {
                var adminId = GetCurrentUserId();
                await _adminService.CreateSettingAsync(dto, adminId);
                TempData["SuccessMessage"] = $"Setting '{dto.SettingKey}' created!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating setting: {ex.Message}";
            }

            return RedirectToAction(nameof(SystemSettings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSetting(UpdateSystemSettingDto dto)
        {
            try
            {
                var adminId = GetCurrentUserId();
                var result = await _adminService.UpdateSettingAsync(dto, adminId);
                TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
                    ? "Setting updated!"
                    : "Setting not found.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating setting: {ex.Message}";
            }

            return RedirectToAction(nameof(SystemSettings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSetting(int settingId)
        {
            try
            {
                var adminId = GetCurrentUserId();
                var result = await _adminService.DeleteSettingAsync(settingId, adminId);
                TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
                    ? "Setting deleted!"
                    : "Setting not found.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting setting: {ex.Message}";
            }

            return RedirectToAction(nameof(SystemSettings));
        }

        #endregion

        #region Audit Logs

        public async Task<IActionResult> AuditLogs(int page = 1, string? search = null, string? action = null, string? entityType = null, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            try
            {
                var result = await _adminService.GetAuditLogsAsync(page, 50, search, action, entityType, dateFrom, dateTo);
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading audit logs: {ex.Message}";
                return View(new AuditLogListDto());
            }
        }

        #endregion
    }
}
