using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Auth;
using PropertyManagementSystem.BLL.Services.Interface;
using System.Security.Claims;

namespace PropertyManagementSystem.Web.Controllers
{
    public class AuthController : Controller
    {
        // Inject AuthService để xử lý logic đăng nhập
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET: /Auth/Login
        // Hiển thị form đăng nhập
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Nếu đã đăng nhập rồi thì về trang chủ
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
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
        {
            ViewData["ReturnUrl"] = returnUrl;

            // Kiểm tra validation
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Gọi service để kiểm tra đăng nhập
            var result = await _authService.LoginAsync(model);

            // Nếu đăng nhập thất bại
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Đăng nhập thất bại");
                return View(model);
            }

            // === ĐĂNG NHẬP THÀNH CÔNG ===

            // Bước 1: Tạo danh sách Claims (thông tin user lưu trong cookie)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.User!.UserId.ToString()), // ID user
                new Claim(ClaimTypes.Email, result.User.Email),                        // Email
                new Claim(ClaimTypes.Name, result.User.FullName),                      // Tên hiển thị
            };

            // Bước 2: Thêm các Role của user vào claims
            foreach (var role in result.User.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Bước 3: Tạo ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Bước 4: Cấu hình cookie
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe, // Ghi nhớ đăng nhập?
                ExpiresUtc = model.RememberMe
                    ? System.DateTimeOffset.UtcNow.AddDays(30)  // Remember: 30 ngày
                    : System.DateTimeOffset.UtcNow.AddHours(8)  // Không remember: 8 tiếng
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
            {
                return Redirect(returnUrl);
            }

            // Không có returnUrl thì redirect theo role
            if (result.User.Roles.Contains("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            if (result.User.Roles.Contains("Landlord"))
            {
                return RedirectToAction("Index", "Landlord");
            }
            if (result.User.Roles.Contains("Technician"))
            {
                return RedirectToAction("Index", "Technician");
            }

            // Mặc định về Home
            return RedirectToAction("Index", "Home");
        }

        // POST: /Auth/Logout
        // Xử lý đăng xuất
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Xóa cookie đăng nhập
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công";
            return RedirectToAction("Login");
        }

        // GET: /Auth/AccessDenied
        // Hiển thị trang không có quyền
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}