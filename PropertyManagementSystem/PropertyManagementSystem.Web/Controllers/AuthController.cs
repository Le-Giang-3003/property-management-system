using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Auth;
using PropertyManagementSystem.BLL.Services.Implementation;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.Web.ViewModels.Auth;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
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
            var dto = new ForgotPasswordRequestDTO
            {
                Email = vm.Email
            };

            var sent = await _authService.SendOtpEmailAsync(dto);
            if (!sent)
            {
                ModelState.AddModelError("", "Email không tồn tại.");
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

    [HttpGet]
    public IActionResult ResetPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("ForgotPassword");

        return View(new ResetPasswordViewModel { Email = email });
    }

    [HttpPost]
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
}
