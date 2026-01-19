using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Auth;
using PropertyManagementSystem.BLL.Interfaces.Auth;

namespace PropertyManagementSystem.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var response = await _authService.LoginAsync(model);

                // Store JWT token in HTTP-only cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = response.ExpiresAt
                };
                Response.Cookies.Append("AuthToken", response.Token, cookieOptions);

                // Store user info in session for display purposes
                HttpContext.Session.SetString("UserEmail", response.Email);
                HttpContext.Session.SetString("UserName", response.FullName);
                HttpContext.Session.SetString("UserRoles", string.Join(",", response.Roles));

                TempData["SuccessMessage"] = $"Welcome back, {response.FullName}!";

                _logger.LogInformation("User {Email} logged in successfully", response.Email);

                // Redirect to return URL or home
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Login failed for {Email}: {Message}", model.Email, ex.Message);
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Delete auth cookie
            Response.Cookies.Delete("AuthToken");

            // Clear session
            HttpContext.Session.Clear();

            TempData["SuccessMessage"] = "You have been logged out successfully.";
            _logger.LogInformation("User logged out");

            return RedirectToAction(nameof(Login));
        }



    }
}
