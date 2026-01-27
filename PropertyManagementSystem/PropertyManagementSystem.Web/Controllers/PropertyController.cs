using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManagementSystem.BLL.DTOs.Property;
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
        private readonly IPropertyImageService _propertyImageService;

        public PropertyController(IPropertyService propertyService, 
            IUserService userService, IFavoritePropertyService favoritePropertyService,
            IDocumentService documentService, IPropertyImageService propertyImageService)
        {
            _propertyService = propertyService;
            _userService = userService;
            _favoritePropertyService = favoritePropertyService;
            _documentService = documentService;
            _propertyImageService = propertyImageService;
        }

        #region Index & Search

        public async Task<IActionResult> Index([FromQuery] PropertySearchDto searchDto)
        {
            // If user is landlord, redirect to MyProperties
            if (User.IsInRole("Landlord") || (User.IsInRole("Member") && !User.IsInRole("Tenant")))
            {
                return RedirectToAction(nameof(MyProperties));
            }

            // Set PortalMode for tenant view
            ViewData["PortalMode"] = "Tenant";

            IEnumerable<Property> properties;

            try
            {
                // ✅ Validate DTO
                if (!searchDto.IsValid(out string errorMessage))
                {
                    TempData["Warning"] = errorMessage;
                    properties = await _propertyService.GetAllPropertiesAsync();
                }
                // ✅ Nếu không có filter, lấy tất cả
                else if (!searchDto.HasFilters())
                {
                    properties = await _propertyService.GetAllPropertiesAsync();
                }
                // ✅ Nếu có filter, gọi search
                else
                {
                    properties = await _propertyService.SearchPropertiesAsync(searchDto);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading list: " + ex.Message;
                properties = new List<Property>();
            }

            // Load favorite status
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

            // values to ViewBag for form retention
            ViewBag.City = searchDto.City;
            ViewBag.PropertyType = searchDto.PropertyType;
            ViewBag.MinRent = searchDto.MinRent;
            ViewBag.MaxRent = searchDto.MaxRent;
            ViewBag.PropertyTypes = GetPropertyTypesSelectList();

            return View("PropertyManagement", properties);
        }

        /// <summary>
        /// Search Properties with card-based UI for tenants
        /// </summary>
        public async Task<IActionResult> SearchProperties([FromQuery] PropertySearchDto searchDto)
        {
            IEnumerable<Property> properties;

            try
            {
                if (!searchDto.IsValid(out string errorMessage))
                {
                    TempData["Warning"] = errorMessage;
                    properties = await _propertyService.GetAllPropertiesAsync();
                }
                else if (!searchDto.HasFilters())
                {
                    properties = await _propertyService.GetAllPropertiesAsync();
                }
                else
                {
                    properties = await _propertyService.SearchPropertiesAsync(searchDto);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading properties: " + ex.Message;
                properties = new List<Property>();
            }

            // Load favorite status
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

            // ViewBag values for form retention
            ViewBag.City = searchDto.City;
            ViewBag.PropertyType = searchDto.PropertyType;
            ViewBag.MinRent = searchDto.MinRent;
            ViewBag.MaxRent = searchDto.MaxRent;
            ViewBag.MinBedrooms = searchDto.MinBedrooms;
            ViewBag.PropertyTypes = GetPropertyTypesSelectList();

            return View("SearchProperties", properties);
        }

        #endregion

        #region Details

        public async Task<IActionResult> Details(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                TempData["Error"] = "Property not found.";
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
            var images = await _propertyImageService.GetImagesByPropertyIdAsync(id);
            ViewBag.PropertyImages = images;

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
                    TempData["Success"] = $"Property '{property.Name}' created successfully!";
                    
                    // Redirect to MyProperties if user is landlord, otherwise Index
                    if (User.IsInRole("Landlord") || User.IsInRole("Member"))
                    {
                        return RedirectToAction(nameof(MyProperties));
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating property: {ex.Message}");
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
                TempData["Error"] = "Property not found.";
                return RedirectToAction(nameof(Index));
            }

            // Authorization
            var currentUserId = GetCurrentUserId();
            if (property.LandlordId != currentUserId && !User.IsInRole("Admin"))
            {
                TempData["Error"] = "You do not have permission to edit this property.";
                return RedirectToAction(nameof(Index));
            }

            // Lấy images từ PropertyImageService thay vì DocumentService
            var images = await _propertyImageService.GetImagesByPropertyIdAsync(id);
            ViewBag.PropertyImages = images;
            ViewBag.PropertyTypes = GetPropertyTypesSelectList();

            return View("PropertyEdit", property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Property property)
        {
            if (id != property.PropertyId)
            {
                TempData["Error"] = "Invalid data.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Authorization
                var currentUserId = GetCurrentUserId();
                if (property.LandlordId != currentUserId && !User.IsInRole("Admin"))
                {
                    TempData["Error"] = "You do not have permission to edit this property.";
                    return RedirectToAction(nameof(Index));
                }

                // Lấy property từ DB
                var existingProperty = await _propertyService.GetPropertyByIdAsync(id);
                if (existingProperty == null)
                {
                    TempData["Error"] = "Property not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra có thay đổi
                if (!HasPropertyChanges(existingProperty, property))
                {
                    TempData["Warning"] = "No changes were made.";
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

                TempData["Success"] = $"Property '{property.Name}' updated successfully!";
                return RedirectToAction(nameof(Details), new { id = property.PropertyId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating: {ex.Message}");
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
                TempData["Error"] = "Property not found.";
                return RedirectToAction(nameof(Index));
            }

            // Authorization
            var currentUserId = GetCurrentUserId();
            if (property.LandlordId != currentUserId && !User.IsInRole("Admin"))
            {
                TempData["Error"] = "You do not have permission to delete this property.";
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
                    TempData["Error"] = "Property not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Authorization
                var currentUserId = GetCurrentUserId();
                if (property.LandlordId != currentUserId && !User.IsInRole("Admin"))
                {
                    TempData["Error"] = "You do not have permission to delete this property.";
                    return RedirectToAction(nameof(Index));
                }

                await _propertyService.DeletePropertyAsync(id);
                TempData["Success"] = $"Property '{property.Name}' deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting property: {ex.Message}";
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
                TempData["Error"] = "Property not found.";
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
                ModelState.AddModelError("NewStatus", "Please select a status.");
                ViewBag.ValidStatuses = new List<string>
                {
                    "Available", "Rented", "Maintenance", "Unavailable"
                };
                return View(vm);
            }

            var property = await _propertyService.GetPropertyByIdAsync(vm.PropertyId);
            if (property == null)
            {
                TempData["Error"] = "Property not found.";
                return RedirectToAction(nameof(Index));
            }

            if (!IsValidStatus(vm.NewStatus))
            {
                ModelState.AddModelError("NewStatus", "Invalid status.");
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
                TempData["Warning"] = "Status has been refreshed.";
            }
            else
            {
                TempData["Success"] = $"Status updated to: {vm.NewStatus}";
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
                    TempData["Error"] = "Please log in";
                    return RedirectToAction("Login", "Account");
                }

                var favorites = await _favoritePropertyService.GetFavoritesByUserIdAsync(userId);
                return View("PropertyFavorite", favorites);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading favorites: " + ex.Message;
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
                    TempData["Error"] = "Please log in";
                    return RedirectToAction("Login", "Account");
                }

                var result = await _favoritePropertyService.RemoveFromFavoriteAsync(userId, propertyId);

                if (result)
                    TempData["Success"] = "Removed from favorites";
                else
                    TempData["Error"] = "Unable to remove";

                // Redirect về trang trước đó hoặc MyFavorites
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction(nameof(MyFavorites));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction(nameof(MyFavorites));
            }
        }

        #endregion

        #region Property Images

        [HttpGet]
        public async Task<IActionResult> ManageImages(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                TempData["Error"] = "Property not found.";
                return RedirectToAction(nameof(Index));
            }

            // Authorization
            var currentUserId = GetCurrentUserId();
            if (property.LandlordId != currentUserId && !User.IsInRole("Admin"))
            {
                TempData["Error"] = "You do not have permission to manage images for this property.";
                return RedirectToAction(nameof(Index));
            }

            var images = await _propertyImageService.GetImagesByPropertyIdAsync(id);
            ViewBag.Property = property;
            ViewBag.PropertyId = id;

            return View("PropertyManageImages", images);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImages(int propertyId, List<IFormFile> files)
        {
            try
            {
                var property = await _propertyService.GetPropertyByIdAsync(propertyId);
                if (property == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy BDS" });
                }

                // Authorization
                var currentUserId = GetCurrentUserId();
                if (property.LandlordId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Json(new { success = false, message = "Không có quyền" });
                }

                if (files == null || !files.Any())
                {
                    return Json(new { success = false, message = "Chưa chọn file" });
                }

                var uploadedImages = await _propertyImageService.UploadMultipleImagesAsync(propertyId, files);

                return Json(new
                {
                    success = true,
                    message = $"✅ Upload thành công {uploadedImages.Count} ảnh",
                    count = uploadedImages.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int imageId, int propertyId)
        {
            try
            {
                var property = await _propertyService.GetPropertyByIdAsync(propertyId);
                if (property == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy BDS" });
                }

                // Authorization
                var currentUserId = GetCurrentUserId();
                if (property.LandlordId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Json(new { success = false, message = "Không có quyền" });
                }

                var result = await _propertyImageService.DeleteImageAsync(imageId, propertyId);

                return Json(new
                {
                    success = result,
                    message = result ? " Đã xóa ảnh" : " Không thể xóa"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetThumbnail(int imageId, int propertyId)
        {
            try
            {
                var property = await _propertyService.GetPropertyByIdAsync(propertyId);
                if (property == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy BDS" });
                }

                // Authorization
                var currentUserId = GetCurrentUserId();
                if (property.LandlordId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Json(new { success = false, message = "Không có quyền" });
                }

                var result = await _propertyImageService.SetThumbnailAsync(propertyId, imageId);

                return Json(new
                {
                    success = result,
                    message = result ? " Đã đặt làm ảnh đại diện" : " Không thể cập nhật"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region My Properties (Landlord)
        /// <summary>
        /// Display all properties of current landlord
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MyProperties()
        {
            // Set PortalMode for landlord
            ViewData["PortalMode"] = "Landlord";

            try
            {
                var userId = GetCurrentUserId();
                if (userId <= 0)
                {
                    TempData["Error"] = "Please log in";
                    return RedirectToAction("Login", "Account");
                }

                var properties = await _propertyService.GetPropertiesByLandlordIdAsync(userId);

                // Load images for each property
                foreach (var property in properties)
                {
                    var images = await _propertyImageService.GetImagesByPropertyIdAsync(property.PropertyId);
                    property.Images = images.ToList();
                }

                ViewBag.TotalProperties = properties.Count();
                ViewBag.AvailableCount = properties.Count(p => p.Status == "Available");
                ViewBag.RentedCount = properties.Count(p => p.Status == "Rented");
                ViewBag.MaintenanceCount = properties.Count(p => p.Status == "Maintenance");

                return View("MyProperties", properties);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading list: " + ex.Message;
                return RedirectToAction("Dashboard", "Home");
            }
        }

        /// <summary>
        /// Quick status update from My Properties page
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickStatusUpdate(int propertyId, string newStatus)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId <= 0)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var property = await _propertyService.GetPropertyByIdAsync(propertyId);
                if (property == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy BDS" });
                }

                // Authorization check
                if (property.LandlordId != userId && !User.IsInRole("Admin"))
                {
                    return Json(new { success = false, message = "Bạn không có quyền cập nhật BDS này" });
                }

                // Validate status
                if (!IsValidStatus(newStatus))
                {
                    return Json(new { success = false, message = "Trạng thái không hợp lệ" });
                }

                // Update status
                property.Status = newStatus;
                property.UpdatedAt = DateTime.UtcNow;
                var result = await _propertyService.UpdatePropertyAsync(property);

                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = $"✅ Đã cập nhật trạng thái: {GetStatusDisplay(newStatus)}",
                        newStatus = newStatus,
                        statusDisplay = GetStatusDisplay(newStatus)
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể cập nhật trạng thái" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        #endregion
        #region Update Caption
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCaption(int imageId, int propertyId, string caption)
        {
            try
            {
                var property = await _propertyService.GetPropertyByIdAsync(propertyId);
                if (property == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy BDS" });
                }

                // Authorization
                var currentUserId = GetCurrentUserId();
                if (property.LandlordId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Json(new { success = false, message = "Không có quyền" });
                }

                var result = await _propertyImageService.UpdateCaptionAsync(imageId, propertyId, caption);

                return Json(new
                {
                    success = result,
                    message = result ? " Đã cập nhật caption" : " Không thể cập nhật"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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

        private string GetStatusDisplay(string status) => status switch
        {
            "Available" => "Có sẵn",
            "Rented" => "Đã thuê",
            "Maintenance" => "Bảo trì",
            "Unavailable" => "Không khả dụng",
            _ => status
        };

        #endregion
    }
}