using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.BLL.DTOs.Auth;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Repositories.Implementation;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly AppDbContext _context;

        public AuthService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            AppDbContext context)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _context = context;
        }

        public async Task<LoginResult> LoginAsync(LoginRequestDto model)
        {
            var user = await _userRepository.GetUserWithRolesByEmailAsync(model.Email);

            if (user == null)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "Email không tồn tại trong hệ thống"
                };
            }

            if (!user.IsActive)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "Tài khoản đã bị vô hiệu hóa"
                };
            }

            if (!_passwordService.VerifyPassword(model.Password, user.PasswordHash))
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "Mật khẩu không chính xác"
                };
            }

            // Cập nhật LastLoginAt - dùng trực tiếp context
            user.LastLoginAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var roles = user.UserRoles?.Select(ur => ur.Role.RoleName).ToList() ?? new List<string>();

            return new LoginResult
            {
                Success = true,
                Message = "Đăng nhập thành công",
                User = new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FullName = user.FullName,
                    Avatar = user.Avatar,
                    Roles = roles
                }
            };
        }

        public Task LogoutAsync()
        {
            return Task.CompletedTask;
        }

        public async Task<UserDto?> GetCurrentUserAsync(int userId)
        {
            // Dùng GetUserByIdAsync thay vì GetUserWithRolesAsync
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return null;

            var roles = user.UserRoles?.Select(ur => ur.Role.RoleName).ToList() ?? new List<string>();

            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Avatar = user.Avatar,
                Roles = roles
            };
        }
    }
}
