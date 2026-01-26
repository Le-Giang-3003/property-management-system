using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Auth;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
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
                ModelState.AddModelError(string.Empty, result.Message ?? "Login failed");
                return View(model);
            }

            //userid = ternanid
            var tenantId = result.User.UserId;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.User!.UserId.ToString()),
                new Claim(ClaimTypes.Email, result.User.Email),
                new Claim(ClaimTypes.Name, result.User.FullName),
                new Claim("TenantId", tenantId.ToString()) //add tenantId claim
            };
            foreach (var role in result.User.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            Console.WriteLine($"LOGIN UserId = {result.User.UserId}");

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

            TempData["SuccessMessage"] = $"Welcome {result.User.FullName}!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Redirect based on primary role
            if (result.User.Roles.Contains("Admin"))
                return RedirectToAction("Index", "Admin");
            if (result.User.Roles.Contains("Technician"))
                return RedirectToAction("TechnicianIndex", "Maintenance");
            if (result.User.Roles.Contains("Member"))
            {
                // Member can be both Tenant and Landlord - default to Tenant view
                return RedirectToAction("TenantIndex", "Maintenance");
            }

            return RedirectToAction("Index", "Home");
        }

        // ===== LOGOUT & ACCESS DENIED =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "You have been logged out successfully";
            return RedirectToAction("Login");
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
                ModelState.AddModelError(nameof(vm.Email), "Email does not exist in the system");
                return View(vm);
            }

            if (!vm.IsOtpSent)
            {
                var dto = new ForgotPasswordRequestDTO { Email = vm.Email };

                var sent = await _authService.SendOtpEmailAsync(dto);
                if (!sent)
                {
                    ModelState.AddModelError("", "Failed to send OTP. Please try again.");
                    return View(vm);
                }

                vm.IsOtpSent = true;
                ModelState.Clear();
                TempData["Message"] = "OTP code has been sent to your email.";
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
                    ModelState.AddModelError("", "OTP is incorrect or has expired.");
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
                ModelState.AddModelError("", "Account not found.");
                return View(vm);
            }

            TempData["SuccessMessage"] = "Password reset successful. Please sign in.";  
            return RedirectToAction("Login", "Auth");
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
                    "Current password is incorrect or new passwords do not match.");
                return View(vm);
            }

            // Logout user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["SuccessMessage"] = "Password changed successfully! Please sign in again.";
            return RedirectToAction("Login");
        }
        


        public IActionResult AccessDenied() => View();
    }
}
