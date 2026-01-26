using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.User;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.Web.Extensions;

namespace PropertyManagementSystem.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        // GET: /Profile
        public async Task<IActionResult> Index()
        {
            var userId = User.GetUserId();
            if (userId == null) return RedirectToAction("Login", "Auth");

            var profile = await _profileService.GetProfileAsync(userId.Value);
            if (profile == null)
            {
                TempData["Error"] = "User not found";
                return RedirectToAction("Index", "Home");
            }
            return View(profile);
        }

        // GET: /Profile/ManageProfile - Modern UI version
        public async Task<IActionResult> ManageProfile()
        {
            var userId = User.GetUserId();
            if (userId == null) return RedirectToAction("Login", "Auth");

            var profile = await _profileService.GetProfileAsync(userId.Value);
            if (profile == null)
            {
                TempData["Error"] = "User not found";
                return RedirectToAction("Index", "Home");
            }
            return View(profile);
        }

        // POST: /Profile/UpdateProfile - AJAX endpoint
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto model)
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Please login" });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            var (success, message) = await _profileService.UpdateProfileAsync(userId.Value, model);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success, message });
            }

            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction("ManageProfile");
        }

        // GET: /Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = User.GetUserId();
            if (userId == null) return RedirectToAction("Login", "Auth");

            var profile = await _profileService.GetProfileAsync(userId.Value);
            if (profile == null) return RedirectToAction("Index");

            return View(new UpdateProfileDto
            {
                FullName = profile.FullName,
                PhoneNumber = profile.PhoneNumber,
                Address = profile.Address,
                DateOfBirth = profile.DateOfBirth,
                Avatar = profile.Avatar
            });
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateProfileDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = User.GetUserId();
            if (userId == null) return RedirectToAction("Login", "Auth");

            var (success, message) = await _profileService.UpdateProfileAsync(userId.Value, model);
            TempData[success ? "Success" : "Error"] = message;

            return success ? RedirectToAction("Index") : View(model);
        }

        // GET: /Profile/Search (Admin/Landlord)
        [Authorize(Roles = "Admin,Landlord")]
        public IActionResult Search() => View(new UserSearchDto());

        // POST: /Profile/Search
        [HttpPost]
        [Authorize(Roles = "Admin,Landlord")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(UserSearchDto searchDto)
        {
            ViewBag.SearchResults = await _profileService.SearchUsersAsync(searchDto);
            return View(searchDto);
        }

        // GET: /Profile/View/{id} (Admin/Landlord)
        [Authorize(Roles = "Admin,Landlord")]
        public async Task<IActionResult> View(int id)
        {
            var profile = await _profileService.GetProfileAsync(id);
            if (profile == null)
            {
                TempData["Error"] = "User not found";
                return RedirectToAction("Search");
            }
            return View(profile);
        }
    }
}
