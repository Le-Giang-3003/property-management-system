using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.Web.ViewModels;

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
        /// <summary>
        /// Initializes a new instance of the <see cref="UserController" /> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel()); // Return an empty model to the view
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

                TempData["Info"] = "Mã OTP đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư.";

                return RedirectToAction("VerifyOtp");
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
                return RedirectToAction("Register");
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

            return View(model);
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
                TempData["Error"] = "Phiên đăng ký đã hết hạn. Vui lòng đăng ký lại.";
                return RedirectToAction("Register");
            }

            // Verify OTP
            var isValid = _userService.VerifyRegistrationOtp(email, vm.Otp, otpHash);

            if (!isValid)
            {
                ModelState.AddModelError("", "Mã OTP không chính xác hoặc đã hết hạn.");
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
                TempData["Error"] = "Đăng ký thất bại. Vui lòng thử lại.";
                return RedirectToAction("Register");
            }

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("NavigateLoginPage","Login");
        }
        /// <summary>
        /// Resends the otp.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendOtp()
        {
            var email = TempData["RegistrationEmail"]?.ToString();

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Register");
            }

            try
            {
                var otpHash = await _userService.SendRegistrationOtpAsync(email);

                TempData["OtpHash"] = otpHash;
                TempData.Keep("RegistrationEmail");
                TempData.Keep("RegistrationUsername");
                TempData.Keep("RegistrationPassword");

                TempData["Success"] = "Mã OTP mới đã được gửi đến email của bạn.";

                return RedirectToAction("VerifyOtp");
            }
            catch (Exception)
            {
                TempData["Error"] = "Không thể gửi lại mã OTP. Vui lòng thử lại.";
                return RedirectToAction("VerifyOtp");
            }
        }
    }
}
