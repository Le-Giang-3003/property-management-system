using PropertyManagementSystem.BLL.DTOs.Auth;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LoginResult> LoginAsync(LoginRequestDto model)
        {
            var user = await _unitOfWork.Users.GetUserWithRolesByEmailAsync(model.Email);

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

            //if (!VerifyPassword(model.Password, user.PasswordHash))
            //{
            //    return new LoginResult
            //    {
            //        Success = false,
            //        Message = "Mật khẩu không chính xác"
            //    };
            //}

            // Cập nhật LastLoginAt
            user.LastLoginAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var roles = user.UserRoles?.Select(ur => ur.Role.RoleName).ToList() ?? new List<string>();

            return new LoginResult
            {
                Success = true,
                Message = "Đăng nhập thành công",
                User = new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Avatar = user.Avatar,
                    Roles = roles
                }
            };
        }

        public Task LogoutAsync()
        {
            // Logout logic được xử lý ở Controller với SignOutAsync
            return Task.CompletedTask;
        }

        public async Task<UserDto?> GetCurrentUserAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetUserWithRolesAsync(userId);
            if (user == null) return null;

            var roles = user.UserRoles?.Select(ur => ur.Role.RoleName).ToList() ?? new List<string>();

            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Avatar = user.Avatar,
                Roles = roles
            };
        }


    }
}
