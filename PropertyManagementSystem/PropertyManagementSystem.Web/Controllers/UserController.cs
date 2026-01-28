using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.Web.ViewModels.User;

namespace PropertyManagementSystem.Web.Controllers
{
    /// <summary>
    /// Controller for user-related actions.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class UserController : Controller
    {
        /// <summary>
        /// The user service
        /// </summary>
        private readonly IUserService _userService;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController" /> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        public UserController(IUserService userService, IMemoryCache memoryCache)
        {
            _userService = userService;
            _cache = memoryCache;
        }
        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel()); // Return Register.cshtml with RegisterViewModel (empty)
        }
        /// <summary>
        /// Registers the specified vm.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (vm.Password != vm.ConfirmPassword)
            {
                ModelState.AddModelError("", "Password and confirm password do not match.");
                return View(vm);
            }

            try
            {
                var otpHash = await _userService.SendRegistrationOtpAsync(vm.Email);
                TempData["RegistrationEmail"] = vm.Email;
                TempData["RegistrationUsername"] = vm.UserName;
                TempData["RegistrationPassword"] = vm.Password;
                TempData["OtpHash"] = otpHash;

                TempData["Info"] = "OTP has been sent to your email. Please check your inbox.";

                return RedirectToAction("VerifyOtp"); // Redirect to OTP verification page
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", $"Error sending OTP: {ex.Message}");
                return View(vm);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while sending OTP. Please try again later.");
                return View(vm);
            }
        }
        /// <summary>
        /// Verifies the otp.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult VerifyOtp()
        {
            if (TempData["RegistrationEmail"] == null)
            {
                return RedirectToAction("Register"); // if no email in TempData, redirect to Register.cshtml
            }

            var model = new VerifyOtpViewModel
            {
                Email = TempData["RegistrationEmail"]?.ToString() ?? "",
                OtpHash = TempData["OtpHash"]?.ToString() ?? ""
            };

            // Keep data for POST
            TempData.Keep("RegistrationEmail");
            TempData.Keep("RegistrationUsername");
            TempData.Keep("RegistrationPassword");
            TempData.Keep("OtpHash");

            return View(model); // Return VerifyOtp.cshtml with VerifyOtpViewModel (have email and otpHash)
        }
        /// <summary>
        /// Verifies the otp.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData.Keep("RegistrationEmail");
                TempData.Keep("RegistrationUsername");
                TempData.Keep("RegistrationPassword");
                TempData.Keep("OtpHash");
                return View(vm);
            }

            var email = TempData["RegistrationEmail"]?.ToString();
            var username = TempData["RegistrationUsername"]?.ToString();
            var password = TempData["RegistrationPassword"]?.ToString();
            var otpHash = TempData["OtpHash"]?.ToString();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(otpHash))
            {
                TempData["Error"] = "Registration session has expired. Please register again.";
                return RedirectToAction("Register");
            }

            // Verify OTP
            var resultOtp = _userService.VerifyRegistrationOtp(email, vm.Otp, otpHash);

            if (!resultOtp.IsValid)
            {
                ModelState.AddModelError("", resultOtp.Message);
                vm.Email = email;
                vm.OtpHash = otpHash;

                TempData.Keep("RegistrationEmail");
                TempData.Keep("RegistrationUsername");
                TempData.Keep("RegistrationPassword");
                TempData.Keep("OtpHash");

                return View(vm);
            }

            // Register user
            var result = await _userService.RegisterWithVerifiedEmail(email, username, password);

            if (!result)
            {
                TempData["Error"] = "Registration failed. Please try again.";
                return RedirectToAction("Register");
            }

            TempData["SuccessMessage"] = "Registration successful! Please sign in.";
            return RedirectToAction("Login", "Auth");
        }
        /// <summary>
        /// Resends the otp.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ResendOtp(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Email is required.";
                return RedirectToAction("Register");
            }

            var cacheKey = $"resend_otp_{email}";
            var lastResendTime = _cache.Get<DateTime?>(cacheKey);

            if (lastResendTime.HasValue)
            {
                var timeSinceLastResend = DateTime.UtcNow - lastResendTime.Value;
                if (timeSinceLastResend.TotalSeconds < 60)
                {
                    var waitTime = 60 - (int)timeSinceLastResend.TotalSeconds;
                    TempData["ErrorMessage"] = $"Please wait {waitTime} seconds before requesting a new code.";

                    var model = new VerifyOtpViewModel
                    {
                        Email = email,
                        OtpHash = TempData["OtpHash"]?.ToString() ?? ""
                    };

                    TempData.Keep("RegistrationEmail");
                    TempData.Keep("RegistrationUsername");
                    TempData.Keep("RegistrationPassword");
                    TempData.Keep("OtpHash");

                    return View("VerifyOtp", model);
                }
            }

            try
            {
                var otpHash = await _userService.SendRegistrationOtpAsync(email);

                _cache.Set(cacheKey, DateTime.UtcNow, TimeSpan.FromMinutes(10));

                TempData["OtpHash"] = otpHash;
                TempData.Keep("RegistrationEmail");
                TempData.Keep("RegistrationUsername");
                TempData.Keep("RegistrationPassword");

                TempData["SuccessMessage"] = "A new verification code has been sent to your email.";

                return RedirectToAction("VerifyOtp");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to resend OTP: {ex.Message}";
                return RedirectToAction("Register");
            }
        }
    }
}
