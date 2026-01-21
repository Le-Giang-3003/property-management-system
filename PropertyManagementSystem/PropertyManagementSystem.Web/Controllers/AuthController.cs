using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Auth;
using PropertyManagementSystem.BLL.Services.Implementation;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.Web.ViewModels.Auth;
using System.Security.Claims;

namespace PropertyManagementSystem.Web.Controllers
{
    public class AuthController : Controller
    {
        // Inject AuthService để xử lý logic đăng nhập
        private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        _userService = userService;
        }

        // GET: /Auth/Login
        // Hiển thị form đăng nhập
        [HttpGet]
    public IActionResult Login() => View();

    [HttpGet]
    public IActionResult ForgotPassword()
        public IActionResult Login(string? returnUrl = null)
        {
            // Nếu đã đăng nhập rồi thì về trang chủ
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
        return View(new ForgotPasswordViewModel());
            }

            // Lưu returnUrl để sau khi login xong redirect về
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Auth/Login
        // Xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken] // Chống CSRF attack
        public async Task<IActionResult> Login(LoginRequestDto model, string? returnUrl = null)
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(vm);

            // Kiểm tra validation
            if (!ModelState.IsValid)
        var user = await _userService.GetUserByEmailAsync(vm.Email);
        if (user == null)
            {
                return View(model);
            ModelState.AddModelError(nameof(vm.Email), "Email không tồn tại trong hệ thống");
            return View(vm);
            }

            // Gọi service để kiểm tra đăng nhập
            var result = await _authService.LoginAsync(model);

            // Nếu đăng nhập thất bại
            if (!result.Success)
        if (!vm.IsOtpSent)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Đăng nhập thất bại");
                return View(model);
            }

            // === ĐĂNG NHẬP THÀNH CÔNG ===

            // Bước 1: Tạo danh sách Claims (thông tin user lưu trong cookie)
            var claims = new List<Claim>
            var dto = new ForgotPasswordRequestDTO
            {
                new Claim(ClaimTypes.NameIdentifier, result.User!.UserId.ToString()), // ID user
                new Claim(ClaimTypes.Email, result.User.Email),                        // Email
                new Claim(ClaimTypes.Name, result.User.FullName),                      // Tên hiển thị
                Email = vm.Email
            };

            // Bước 2: Thêm các Role của user vào claims
            foreach (var role in result.User.Roles)
            var sent = await _authService.SendOtpEmailAsync(dto);
            if (!sent)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                ModelState.AddModelError("", "Email không tồn tại.");
                return View(vm);
            }

            // Bước 3: Tạo ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Bước 4: Cấu hình cookie
            var authProperties = new AuthenticationProperties
            vm.IsOtpSent = true;
            ModelState.Clear();
            TempData["Message"] = "Mã OTP đã được gửi tới email của bạn.";
            return View(vm);
        }
        else
        {
            var verifyDto = new VerifyOtpRequestDTO
            {
                IsPersistent = model.RememberMe, // Ghi nhớ đăng nhập?
                ExpiresUtc = model.RememberMe
                    ? System.DateTimeOffset.UtcNow.AddDays(30)  // Remember: 30 ngày
                    : System.DateTimeOffset.UtcNow.AddHours(8)  // Không remember: 8 tiếng
                Email = vm.Email,
                OtpCode = vm.OtpCode
            };

            // Bước 5: Thực hiện đăng nhập (ghi cookie)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            TempData["SuccessMessage"] = $"Chào mừng {result.User.FullName}!";

            // Bước 6: Redirect
            // Nếu có returnUrl thì về đó
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            var (isValid, userId) = await _authService.VerifyOtpAsync(verifyDto);
            if (!isValid)
            {
                return Redirect(returnUrl);
                ModelState.AddModelError("", "OTP không đúng hoặc đã hết hạn.");
                return View(vm);
            }

            // Không có returnUrl thì redirect theo role
            if (result.User.Roles.Contains("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            return RedirectToAction("ResetPassword", new { email = vm.Email });
            }
            if (result.User.Roles.Contains("Landlord"))
            {
                return RedirectToAction("Index", "Landlord");
            }
            if (result.User.Roles.Contains("Technician"))

    [HttpGet]
    public IActionResult ResetPassword(string email)
            {
                return RedirectToAction("Index", "Technician");
            }
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("ForgotPassword");

            // Mặc định về Home
            return RedirectToAction("Index", "Home");
        return View(new ResetPasswordViewModel { Email = email });
        }

        // POST: /Auth/Logout
        // Xử lý đăng xuất
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            // Xóa cookie đăng nhập
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!ModelState.IsValid) return View(vm);

            TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công";
            return RedirectToAction("Login");
        }
        var dto = new ResetPasswordRequestDTO
        {
            Email = vm.Email,
            NewPassword = vm.NewPassword,
            ConfirmPassword = vm.ConfirmPassword
        };

        // GET: /Auth/AccessDenied
        // Hiển thị trang không có quyền
        public IActionResult AccessDenied()
        var ok = await _authService.ResetPasswordAsync(dto);
        if (!ok)
        {
            return View();
            ModelState.AddModelError("", "Không tìm thấy tài khoản.");
            return View(vm);
        }

        TempData["Success"] = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập.";
        return RedirectToAction("Login", "Auth");
    }
}