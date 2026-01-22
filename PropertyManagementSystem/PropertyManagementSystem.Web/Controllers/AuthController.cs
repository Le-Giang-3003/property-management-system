using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Auth;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.Web.ViewModels.Auth;
using System.Security.Claims;

namespace PropertyManagementSystem.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        // ===== LOGIN =====
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var result = await _authService.LoginAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Đăng nhập thất bại");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.User!.UserId.ToString()),
                new Claim(ClaimTypes.Email, result.User.Email),
                new Claim(ClaimTypes.Name, result.User.FullName),
            };
            foreach (var role in result.User.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(30)
                    : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            TempData["SuccessMessage"] = $"Chào mừng {result.User.FullName}!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            if (result.User.Roles.Contains("Admin"))
                return RedirectToAction("Index", "Admin");
            if (result.User.Roles.Contains("Landlord"))
                return RedirectToAction("Index", "Landlord");
            if (result.User.Roles.Contains("Technician"))
                return RedirectToAction("Index", "Technician");

            return RedirectToAction("Index", "Home");
        }

        // ===== FORGOT PASSWORD =====
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userService.GetUserByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(vm.Email), "Email không tồn tại trong hệ thống");
                return View(vm);
            }

            if (!vm.IsOtpSent)
            {
                var dto = new ForgotPasswordRequestDTO { Email = vm.Email };

                var sent = await _authService.SendOtpEmailAsync(dto);
                if (!sent)
                {
                    ModelState.AddModelError("", "Không gửi được OTP. Vui lòng thử lại.");
                    return View(vm);
                }

                vm.IsOtpSent = true;
                ModelState.Clear();
                TempData["Message"] = "Mã OTP đã được gửi tới email của bạn.";
                return View(vm);
            }
            else
            {
                var verifyDto = new VerifyOtpRequestDTO
                {
                    Email = vm.Email,
                    OtpCode = vm.OtpCode
                };

                var (isValid, userId) = await _authService.VerifyOtpAsync(verifyDto);
                if (!isValid)
                {
                    ModelState.AddModelError("", "OTP không đúng hoặc đã hết hạn.");
                    return View(vm);
                }

                return RedirectToAction("ResetPassword", new { email = vm.Email });
            }
        }

        // ===== RESET PASSWORD =====
        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("ForgotPassword");

            return View(new ResetPasswordViewModel { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var dto = new ResetPasswordRequestDTO
            {
                Email = vm.Email,
                NewPassword = vm.NewPassword,
                ConfirmPassword = vm.ConfirmPassword
            };

            var ok = await _authService.ResetPasswordAsync(dto);
            if (!ok)
            {
                ModelState.AddModelError("", "Không tìm thấy tài khoản.");
                return View(vm);
            }

            TempData["Success"] = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập.";
            return RedirectToAction("Login", "Auth");
        }

        // ===== LOGOUT & ACCESS DENIED =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var dto = new ChangePasswordRequestDTO
            {
                CurrentPassword = vm.CurrentPassword,
                NewPassword = vm.NewPassword,
                ConfirmPassword = vm.ConfirmPassword
            };

            var result = await _authService.ChangePasswordAsync(email, dto);
            if (!result)
            {
                ModelState.AddModelError(string.Empty,
                    "Mật khẩu hiện tại không đúng hoặc mật khẩu mới không khớp.");
                return View(vm);
            }

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Login");
        }
        public IActionResult AccessDenied() => View();
    }
}
