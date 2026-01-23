using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.Web.ViewModels.Property;

namespace PropertyManagementSystem.Web.Controllers
{
    public class PropertyController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IUserService _userService;
        private readonly IFavoritePropertyService _favoritePropertyService;
        private readonly IDocumentService _documentService;

        public PropertyController(IPropertyService propertyService, 
            IUserService userService, IFavoritePropertyService favoritePropertyService, IDocumentService documentService)
        {
            _propertyService = propertyService;
            _userService = userService;
            _favoritePropertyService = favoritePropertyService;
            _documentService = documentService;
        }

        #region Index & Search

        public async Task<IActionResult> Index(string? city = null, string? propertyType = null,
            decimal? minRent = null, decimal? maxRent = null)
        {
            IEnumerable<Property> properties;

            try
            {
                if (string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(propertyType))
                {
                    properties = await _propertyService.GetAllPropertiesAsync();
                }
                else
                {
                    properties = await _propertyService.SearchPropertiesAsync(
                        city ?? "", propertyType ?? "", minRent, maxRent);
                }
            }
            catch (ArgumentException ex) when (ex.Message.Contains("City is required"))
            {
                properties = await _propertyService.GetAllPropertiesAsync();
                TempData["Warning"] = "Bộ lọc không hợp lệ, hiển thị tất cả BDS.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi tải danh sách: " + ex.Message;
                properties = new List<Property>();
            }

            // Load favorite status nếu user đã đăng nhập
            var userId = GetCurrentUserId();
            if (userId > 0)
            {
                var favorites = await _favoritePropertyService.GetFavoritesByUserIdAsync(userId);
                ViewBag.FavoritePropertyIds = favorites.Select(f => f.PropertyId).ToHashSet();
            }
            else
            {
                ViewBag.FavoritePropertyIds = new HashSet<int>();
            }

            ViewBag.City = city;
            ViewBag.PropertyType = propertyType;
            ViewBag.MinRent = minRent;
            ViewBag.MaxRent = maxRent;
            ViewBag.PropertyTypes = GetPropertyTypesSelectList();

            return View("PropertyManagement", properties);
        }

        #endregion

        #region Details

        public async Task<IActionResult> Details(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                TempData["Error"] = "Không tìm thấy BDS.";
                return RedirectToAction(nameof(Index));
            }

            // Check nếu property này đã được favorite chưa
            var userId = GetCurrentUserId();
            if (userId>0)
            {
                ViewBag.IsFavorited = await _favoritePropertyService.IsPropertyFavoritedAsync(userId, id);
            }
            else
            {
                ViewBag.IsFavorited = false;
            }
            var documents = await _documentService.GetDocumentsByEntityAsync("Property", id);
            ViewBag.Documents = documents;

            return View("PropertyDetail", property);
        }

        #endregion

        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new CreatePropertyViewModel
            {
                PropertyTypes = GetPropertyTypesSelectList(),
                AvailableLandlords = await GetLandlordsSelectList(),
                AvailableFrom = DateTime.Today,
                Bedrooms = 1,
                Bathrooms = 1,
                SquareFeet = 50
            };
            return View("PropertyCreate", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePropertyViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var property = new Property
                    {
                        // Basic Info
                        Name = vm.Name,

                        // Location
                        Address = vm.Address,
                        City = vm.City ?? string.Empty,
                        District = vm.District ?? string.Empty,
                        ZipCode = vm.ZipCode ?? string.Empty,
                        Latitude = vm.Latitude,
                        Longitude = vm.Longitude,

                        // Type & Specs
                        PropertyType = vm.PropertyType,
                        Bedrooms = vm.Bedrooms,
                        Bathrooms = vm.Bathrooms,
                        SquareFeet = vm.SquareFeet,

                        // Pricing
                        RentAmount = vm.RentAmount,
                        DepositAmount = vm.DepositAmount,

                        // Description & Features
                        Description = vm.Description ?? string.Empty,
                        Amenities = vm.Amenities ?? string.Empty,
                        UtilitiesIncluded = vm.UtilitiesIncluded ?? string.Empty,

                        IsFurnished = vm.IsFurnished,
                        PetsAllowed = vm.PetsAllowed,
                        AvailableFrom = vm.AvailableFrom ?? DateTime.Today,

                        // System Fields
                        LandlordId = GetCurrentUserId(),
                        Status = "Available",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _propertyService.AddPropertyAsync(property);
                    TempData["Success"] = $"✅ Tạo BDS '{property.Name}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"❌ Lỗi tạo BDS: {ex.Message}");
                }
            }

            // Reload dropdowns
            vm.PropertyTypes = GetPropertyTypesSelectList();
            vm.AvailableLandlords = await GetLandlordsSelectList();
            return View("PropertyCreate", vm);
        }

        #endregion

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                TempData["Error"] = "Không tìm thấy BDS.";
                return RedirectToAction(nameof(Index));
            }

            // Authorization
            var currentUserId = GetCurrentUserId();
            if (property.LandlordId != currentUserId && !User.IsInRole("Admin"))
            {
                TempData["Error"] = "Bạn không có quyền sửa BDS này.";
                return RedirectToAction(nameof(Index));
            }

            var documents = await _documentService.GetDocumentsByEntityAsync("Property", id);
            ViewBag.Documents = documents;
            ViewBag.PropertyTypes = GetPropertyTypesSelectList();
            return View("PropertyEdit", property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Property property)
        {
            if (id != property.PropertyId)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Authorization
                var currentUserId = GetCurrentUserId();
                if (property.LandlordId != currentUserId && !User.IsInRole("Admin"))
                {
                    TempData["Error"] = "Bạn không có quyền sửa BDS này.";
                    return RedirectToAction(nameof(Index));
                }

                // Lấy property từ DB
                var existingProperty = await _propertyService.GetPropertyByIdAsync(id);
                if (existingProperty == null)
                {
                    TempData["Error"] = "Không tìm thấy BDS.";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra có thay đổi
                if (!HasPropertyChanges(existingProperty, property))
                {
                    TempData["Warning"] = "⚠️ Không có thay đổi nào được thực hiện.";
                    return RedirectToAction(nameof(Details), new { id = property.PropertyId });
                }

                // Validate chỉ khi có thay đổi
                if (!ModelState.IsValid)
                {
                    ViewBag.PropertyTypes = GetPropertyTypesSelectList();
                    return View("PropertyEdit", property);
                }

                // Update
                property.UpdatedAt = DateTime.UtcNow;
                await _propertyService.UpdatePropertyAsync(property);

                TempData["Success"] = $"✅ Cập nhật '{property.Name}' thành công!";
                return RedirectToAction(nameof(Details), new { id = property.PropertyId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"❌ Lỗi cập nhật: {ex.Message}");
                ViewBag.PropertyTypes = GetPropertyTypesSelectList();
                return View("PropertyEdit", property);
            }
        }

        // Helper Method
        private bool HasPropertyChanges(Property existing, Property updated)
        {
            return existing.Name != updated.Name ||
                   existing.Address != updated.Address ||
                   existing.City != updated.City ||
                   existing.District != updated.District ||
                   existing.ZipCode != updated.ZipCode ||
                   existing.PropertyType != updated.PropertyType ||
                   existing.Bedrooms != updated.Bedrooms ||
                   existing.Bathrooms != updated.Bathrooms ||
                   existing.SquareFeet != updated.SquareFeet ||
                   existing.RentAmount != updated.RentAmount ||
                   existing.DepositAmount != updated.DepositAmount ||
                   existing.Description != updated.Description ||
                   existing.Amenities != updated.Amenities ||
                   existing.UtilitiesIncluded != updated.UtilitiesIncluded ||
                   existing.IsFurnished != updated.IsFurnished ||
                   existing.PetsAllowed != updated.PetsAllowed ||
                   existing.AvailableFrom != updated.AvailableFrom ||
                   existing.Status != updated.Status ||
                   existing.Latitude != updated.Latitude ||
                   existing.Longitude != updated.Longitude;
        }


        #endregion

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                TempData["Error"] = "Không tìm thấy BDS.";
                return RedirectToAction(nameof(Index));
            }

            // Authorization
            var currentUserId = GetCurrentUserId();
            if (property.LandlordId != currentUserId && !User.IsInRole("Admin"))
            {
                TempData["Error"] = "Bạn không có quyền xóa BDS này.";
                return RedirectToAction(nameof(Index));
            }

            return View("PropertyDelete", property);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var property = await _propertyService.GetPropertyByIdAsync(id);
                if (property == null)
                {
                    TempData["Error"] = "Không tìm thấy BDS.";
                    return RedirectToAction(nameof(Index));
                }

                // Authorization
                var currentUserId = GetCurrentUserId();
                if (property.LandlordId != currentUserId && !User.IsInRole("Admin"))
                {
                    TempData["Error"] = "Bạn không có quyền xóa BDS này.";
                    return RedirectToAction(nameof(Index));
                }

                await _propertyService.DeletePropertyAsync(id);
                TempData["Success"] = $"✅ Đã xóa '{property.Name}' thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Lỗi xóa BDS: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Change Status

        [HttpGet]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                TempData["Error"] = "Không tìm thấy BDS.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new ChangePropertyStatusViewModel
            {
                PropertyId = property.PropertyId,
                Name = property.Name,
                CurrentStatus = property.Status,
                NewStatus = property.Status
            };

            ViewBag.ValidStatuses = new List<string>
            {
                "Available", "Rented", "Maintenance", "Unavailable"
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(ChangePropertyStatusViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.NewStatus))
            {
                ModelState.AddModelError("NewStatus", "Vui lòng chọn trạng thái.");
                ViewBag.ValidStatuses = new List<string>
                {
                    "Available", "Rented", "Maintenance", "Unavailable"
                };
                return View(vm);
            }

            var property = await _propertyService.GetPropertyByIdAsync(vm.PropertyId);
            if (property == null)
            {
                TempData["Error"] = "Không tìm thấy BDS.";
                return RedirectToAction(nameof(Index));
            }

            if (!IsValidStatus(vm.NewStatus))
            {
                ModelState.AddModelError("NewStatus", "Trạng thái không hợp lệ.");
                ViewBag.ValidStatuses = new List<string>
                {
                    "Available", "Rented", "Maintenance", "Unavailable"
                };
                return View(vm);
            }

            property.Status = vm.NewStatus;
            property.UpdatedAt = DateTime.UtcNow;
            await _propertyService.UpdatePropertyAsync(property);

            if (vm.NewStatus == vm.CurrentStatus)
            {
                TempData["Warning"] = "⚠️ Trạng thái được làm mới.";
            }
            else
            {
                TempData["Success"] = $"✅ Đã cập nhật trạng thái: {vm.NewStatus}";
            }

            return RedirectToAction(nameof(Details), new { id = vm.PropertyId });
        }



        #endregion

        #region Favorite Actions

        /// <summary>
        /// Toggle favorite - AJAX endpoint
        /// </summary>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int propertyId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId<0)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });
                }

                // Check property có tồn tại không
                var property = await _propertyService.GetPropertyByIdAsync(propertyId);
                if (property == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy BDS" });
                }

                var result = await _favoritePropertyService.ToggleFavoriteAsync(userId, propertyId);
                var isFavorited = await _favoritePropertyService.IsPropertyFavoritedAsync(userId, propertyId);

                return Json(new
                {
                    success = result,
                    isFavorited = isFavorited,
                    message = isFavorited ? "✅ Đã thêm vào yêu thích" : "❌ Đã xóa khỏi yêu thích"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        /// <summary>
        /// Hiển thị danh sách BDS yêu thích của user
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyFavorites()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId < 0)
                {
                    TempData["Error"] = "Vui lòng đăng nhập";
                    return RedirectToAction("Login", "Account");
                }

                var favorites = await _favoritePropertyService.GetFavoritesByUserIdAsync(userId);
                return View("PropertyFavorite", favorites);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi tải danh sách yêu thích: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Xóa BDS khỏi danh sách yêu thích
        /// </summary>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFavorite(int propertyId, string? returnUrl = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId < 0)
                {
                    TempData["Error"] = "Vui lòng đăng nhập";
                    return RedirectToAction("Login", "Account");
                }

                var result = await _favoritePropertyService.RemoveFromFavoriteAsync(userId, propertyId);

                if (result)
                    TempData["Success"] = "✅ Đã xóa khỏi danh sách yêu thích";
                else
                    TempData["Error"] = "❌ Không thể xóa";

                // Redirect về trang trước đó hoặc MyFavorites
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction(nameof(MyFavorites));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToAction(nameof(MyFavorites));
            }
        }

        #endregion

        #region Helper Methods

        private List<SelectListItem> GetPropertyTypesSelectList()
        {
            return new List<SelectListItem>
            {
                new() { Value = "Apartment", Text = "Căn hộ (Apartment)" },
                new() { Value = "House", Text = "Nhà riêng (House)" },
                new() { Value = "Condo", Text = "Căn hộ chung cư (Condo)" },
                new() { Value = "Studio", Text = "Studio" },
                new() { Value = "Villa", Text = "Biệt thự (Villa)" },
                new() { Value = "Office", Text = "Văn phòng (Office)" }
            };
        }

        private async Task<List<SelectListItem>> GetLandlordsSelectList()
        {
            var landlords = await _userService.GetUsersByRoleAsync("Landlord");
            return landlords.Select(l => new SelectListItem
            {
                Value = l.UserId.ToString(),
                Text = l.FullName
            }).ToList();
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value ??
                         User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        private bool IsValidStatus(string status)
        {
            string[] validStatuses = { "Available", "Rented", "Maintenance", "Unavailable" };
            return validStatuses.Contains(status);
        }

        #endregion
    }
}