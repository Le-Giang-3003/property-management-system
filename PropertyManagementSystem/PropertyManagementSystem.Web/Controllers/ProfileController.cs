using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.User;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.Web.Extensions;
using System.IO;

namespace PropertyManagementSystem.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly IWebHostEnvironment _env;

        public ProfileController(IProfileService profileService, IWebHostEnvironment env)
        {
            _profileService = profileService;
            _env = env;
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
        public async Task<IActionResult> Edit(UpdateProfileDto model, IFormFile? avatarFile)
        {
            var userId = User.GetUserId();
            if (userId == null) return RedirectToAction("Login", "Auth");

            // Get current profile to check existing avatar
            var currentProfile = await _profileService.GetProfileAsync(userId.Value);
            var hadPreviousAvatar = currentProfile != null && !string.IsNullOrEmpty(currentProfile.Avatar);

            // Handle photo removal (when Avatar is explicitly cleared and no new file uploaded)
            if (avatarFile == null && string.IsNullOrEmpty(model.Avatar) && hadPreviousAvatar && currentProfile != null && currentProfile.Avatar.StartsWith("/uploads/profiles/"))
            {
                var oldFilePath = System.IO.Path.Combine(_env.WebRootPath, currentProfile.Avatar.TrimStart('/').Replace('/', System.IO.Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(oldFilePath))
                {
                    try
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    catch
                    {
                        // Ignore deletion errors
                    }
                }
                model.Avatar = null;
            }

            // Handle file upload
            if (avatarFile != null && avatarFile.Length > 0)
            {
                try
                {
                    // Validate file
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                    var extension = System.IO.Path.GetExtension(avatarFile.FileName).ToLower();
                    
                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["Error"] = "Only image files (jpg, jpeg, png, webp, gif) are allowed";
                        return View(model);
                    }

                    if (avatarFile.Length > 5 * 1024 * 1024) // 5MB
                    {
                        TempData["Error"] = "File size must not exceed 5MB";
                        return View(model);
                    }

                    // Create directory
                    var uploadPath = System.IO.Path.Combine(_env.WebRootPath, "uploads", "profiles", userId.Value.ToString());
                    if (!System.IO.Directory.Exists(uploadPath))
                        System.IO.Directory.CreateDirectory(uploadPath);

                    // Delete old avatar if exists
                    if (hadPreviousAvatar && currentProfile != null && currentProfile.Avatar.StartsWith("/uploads/profiles/"))
                    {
                        var oldFilePath = System.IO.Path.Combine(_env.WebRootPath, currentProfile.Avatar.TrimStart('/').Replace('/', System.IO.Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            try
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                            catch
                            {
                                // Ignore deletion errors
                            }
                        }
                    }

                    // Generate filename
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = System.IO.Path.Combine(uploadPath, fileName);

                    // Save file
                    using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(stream);
                    }

                    // Update avatar URL
                    model.Avatar = $"/uploads/profiles/{userId.Value}/{fileName}";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error uploading file: {ex.Message}";
                    return View(model);
                }
            }

            if (!ModelState.IsValid) return View(model);

            var (success, message) = await _profileService.UpdateProfileAsync(userId.Value, model);
            TempData[success ? "Success" : "Error"] = message;

            return success ? RedirectToAction("ManageProfile") : View(model);
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

        // GET: /Profile/GetAvatar - API endpoint to get user avatar
        [HttpGet]
        public async Task<IActionResult> GetAvatar(int? userId = null)
        {
            var targetUserId = userId ?? User.GetUserId();
            if (targetUserId == null)
            {
                return Json(new { avatar = (string?)null });
            }

            var profile = await _profileService.GetProfileAsync(targetUserId.Value);
            return Json(new { avatar = profile?.Avatar });
        }
    }
}
