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
        /// Initializes a new instance of the <see cref="UserController"/> class.
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

            var result = await _userService.Register(vm.Email, vm.UserName, vm.Password);
            if (!result)
            {
                ModelState.AddModelError("", "Email already exists.");
                return View(vm);
            }

            TempData["Success"] = "Register successfully!";
            return RedirectToAction("Index", "Property"); // Redirect to Property controller's Index action
        }
    }
}
