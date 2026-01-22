using Microsoft.Extensions.Caching.Memory;
using PropertyManagementSystem.BLL.DTOs.Auth;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;
using System.Security.Cryptography;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGenericRepository<User> _userGenericRepository;
        private readonly IPasswordService _passwordService;
        private readonly IMemoryCache _cache;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AuthService(
            IUserRepository userRepository,
            IGenericRepository<User> userGenericRepository,
            IPasswordService passwordService,
            AppDbContext context, IMemoryCache cache, IEmailService emailService)
        {
            _userRepository = userRepository;
            _userGenericRepository = userGenericRepository;
            _passwordService = passwordService;
            _context = context;
            _cache = cache;
            _emailService = emailService;
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
            var user = await _userGenericRepository.GetByIdAsync(userId);
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

        //phat
        public async Task<bool> SendOtpEmailAsync(ForgotPasswordRequestDTO request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                return false;
            }

            var otpCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            _cache.Set(
                $"OTP_{request.Email}",
                (otpCode, user.UserId, attemptCount: 0),
                TimeSpan.FromMinutes(5)
            );

            try
            {
                await _emailService.SendEmailAsync(user.Email, "Mã OTP đặt lại mật khẩu", $"Mã OTP của bạn là: {otpCode}");
                Console.WriteLine($"✅ OTP đã gửi đến: {user.Email}");
                Console.WriteLine($"🔐 Mã OTP (dev): {otpCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi gửi email: {ex.Message}");
                _cache.Remove($"OTP_{request.Email}");
                return false;
            }

            return true;
        }
        public async Task<(bool isValid, int userId)> VerifyOtpAsync(VerifyOtpRequestDTO request)
        {
            var cacheKey = $"OTP_{request.Email}";

            if (!_cache.TryGetValue(cacheKey, out (string otpCode, int userId, int attemptCount) data))
            {
                return (false, 0);
            }

            if (data.attemptCount >= 5)
            {
                _cache.Remove(cacheKey);
                Console.WriteLine($"⚠️ OTP bị khóa do thử quá 5 lần: {request.Email}");
                return (false, 0);
            }

            if (data.otpCode != request.OtpCode)
            {
                _cache.Set(cacheKey, (data.otpCode, data.userId, data.attemptCount + 1), TimeSpan.FromMinutes(5));
                Console.WriteLine($"❌ OTP sai. Lần thử: {data.attemptCount + 1}/5");
                return (false, 0);
            }

            _cache.Remove(cacheKey);
            Console.WriteLine($"✅ OTP xác thực thành công: {request.Email}");

            return (true, data.userId);
        }
        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDTO request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null) return false;

            var newHash = _passwordService.HashPassword(request.NewPassword);
            user.PasswordHash = newHash;

            await _userRepository.UpdateUserAsync(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(string email, ChangePasswordRequestDTO request)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return false;

            // 1. Kiểm tra mật khẩu hiện tại
            if (!_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                Console.WriteLine($"ChangePassword failed: wrong current password for {email}");
                return false;
            }

            // 2. Kiểm tra mật khẩu mới và confirm
            if (request.NewPassword != request.ConfirmPassword)
            {
                Console.WriteLine($"ChangePassword failed: new password and confirm do not match for {email}");
                return false;
            }

            // 3. Hash và lưu mật khẩu mới
            var oldHash = user.PasswordHash;
            var newHash = _passwordService.HashPassword(request.NewPassword);
            user.PasswordHash = newHash;

            await _userRepository.UpdateUserAsync(user);

            Console.WriteLine($"ChangePassword success: email={user.Email}, old={oldHash}, new={user.PasswordHash}");
            return true;
        }


    }
}
