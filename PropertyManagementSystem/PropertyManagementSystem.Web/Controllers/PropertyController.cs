using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.Web.ViewModels.Property;

namespace PropertyManagementSystem.Web.Controllers
{
    /// <summary>
    /// Controller for managing properties.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class PropertyController : Controller
    {
        /// <summary>
        /// The property service
        /// </summary>
        private readonly IPropertyService _propertyService;
        /// <summary>
        /// The user service
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyController" /> class.
        /// </summary>
        /// <param name="propertyService">The property service.</param>
        /// <param name="userService">The user service.</param>
        public PropertyController(IPropertyService propertyService, IUserService userService)
        {
            _propertyService = propertyService;
            _userService = userService;
        }

        // GET: Property/Index
        /// <summary>
        /// Indexes the specified city.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="minRent">The minimum rent.</param>
        /// <param name="maxRent">The maximum rent.</param>
        /// <returns></returns>
        // PropertyController.cs - Index method
        public async Task<IActionResult> Index(string? city = null, string? propertyType = null, decimal? minRent = null, decimal? maxRent = null)
        {
            IEnumerable<Property> properties;

            try
            {
                // Nếu KHÔNG có filter → Load tất cả
                if (string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(propertyType))
                {
                    properties = await _propertyService.GetAllPropertiesAsync();
                }
                else
                {
                    // Có filter → Search
                    properties = await _propertyService.SearchPropertiesAsync(
                        city ?? "",
                        propertyType ?? "",
                        minRent,
                        maxRent);
                }
            }
            catch (ArgumentException ex) when (ex.Message.Contains("City is required"))
            {
                // Fallback khi service strict
                properties = await _propertyService.GetAllPropertiesAsync();
                TempData["Warning"] = "Bộ lọc không hợp lệ, hiển thị tất cả BDS.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi tải danh sách: " + ex.Message;
                properties = new List<Property>();
            }

            // ViewBag cho filter form
            ViewBag.City = city;
            ViewBag.PropertyType = propertyType;
            ViewBag.MinRent = minRent;
            ViewBag.MaxRent = maxRent;
            ViewBag.PropertyTypes = GetPropertyTypesSelectList();

            return View("PropertyManagement",properties);
        }

        // GET: Property/Details/{id}
        /// <summary>
        /// Detailses the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return View("PropertyDetail", property);
        }

        /// <summary>
        /// Gets the landlords select list.
        /// </summary>
        /// <returns></returns>
        private async Task<List<SelectListItem>> GetLandlordsSelectList()
        {
            var landlords = await _userService.GetUsersByRoleAsync("Landlord");
            return landlords.Select(l => new SelectListItem
            {
                Value = l.UserId.ToString(),
                Text = l.FullName
            }).ToList();
        }

        /// <summary>
        /// Gets the property types select list.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the current user identifier.
        /// </summary>
        /// <returns></returns>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value ??
                         User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }


        // GET: Property/Create
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Create()
        {
            var vm = new CreatePropertyViewModel
            {
                AvailableLandlords = await GetLandlordsSelectList(),
                PropertyTypes = GetPropertyTypesSelectList()
            };
            return View("PropertyCreate", vm);
        }

        // POST: Property/Create
        /// <summary>
        /// Creates the specified vm.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
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
                        // Core Info
                        Name = vm.Name,
                        Address = vm.Address,
                        City = vm.City,
                        District = vm.District,
                        ZipCode = vm.ZipCode,

                        // Location
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

                        // Description
                        Description = vm.Description,
                        Amenities = vm.Amenities,
                        UtilitiesIncluded = vm.UtilitiesIncluded,

                        // Features
                        IsFurnished = vm.IsFurnished,
                        PetsAllowed = vm.PetsAllowed,
                        AvailableFrom = vm.AvailableFrom,

                        // FK & Timestamps
                        LandlordId = GetCurrentUserId(),
                        Status = "Available",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _propertyService.AddPropertyAsync(property);
                    TempData["Success"] = " Tạo bất động sản thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi tạo BDS: {ex.Message}");
                }
            }

            // Reload dropdowns khi lỗi
            vm.AvailableLandlords = await GetLandlordsSelectList();
            vm.PropertyTypes = GetPropertyTypesSelectList();
            return View("PropertyCreate", vm);
        }

        /// <summary>
        /// Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null) return NotFound();

            // Authorization
            var currentUserId = GetCurrentUserId();
            if (property.LandlordId != currentUserId)
                return Forbid("Chỉ chủ BDS mới sửa được.");

            ViewBag.PropertyTypes = GetPropertyTypesSelectList();
            return View("PropertyEdit", property);
        }

        /// <summary>
        /// Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Property property)
        {
            if (id != property.PropertyId) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    // Authorization
                    if (property.LandlordId != GetCurrentUserId())
                        return Forbid();

                    property.UpdatedAt = DateTime.UtcNow;
                    await _propertyService.UpdatePropertyAsync(property);

                    TempData["Success"] = $"✅ Cập nhật '{property.Name}' thành công!";
                    return RedirectToAction(nameof(Details), new { id = property.PropertyId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi cập nhật: {ex.Message}");
                }
            }

            ViewBag.PropertyTypes = GetPropertyTypesSelectList();
            return View("PropertyEdit", property);
        }



        // GET: Property/Delete/{id}
        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return View("PropertyDelete", property);
        }

        // POST: Property/Delete/{id}
        /// <summary>
        /// Deletes the confirmed.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _propertyService.DeletePropertyAsync(id);
                TempData["Success"] = "Property deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        /// <summary>
        /// Changes the status.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                TempData["Error"] = "Không có quyền truy cập.";
                return RedirectToAction("Index"); // Redirect to property list if property not found
            }
            var vm = new ChangePropertyStatusViewModel
            {
                PropertyId = property.PropertyId,
                Name = property.Name,
                CurrentStatus = property.Status,
                NewStatus = property.Status
            };
            ViewBag.ValidStatuses = new List<string> { "Available", "Rented", "Maintenance", "Unavailable" }; // ViewBag used to populate dropdown in the view
            return View(vm);
        }
        /// <summary>
        /// Changes the status.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(ChangePropertyStatusViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.NewStatus))
            {
                ModelState.AddModelError("NewStatus", "Vui lòng chọn trạng thái.");
                return View(vm);
            }

            var property = await _propertyService.GetPropertyByIdAsync(vm.PropertyId);
            if (property == null) return NotFound();

            // Client đã validate rồi, server chỉ check valid statuses
            if (!IsValidStatusTransition(vm.NewStatus))
            {
                ModelState.AddModelError("NewStatus", "Trạng thái không hợp lệ.");
                return View(vm);
            }

            // Update (có thể giống hoặc khác đều OK)
            property.Status = vm.NewStatus;
            property.UpdatedAt = DateTime.UtcNow;

            await _propertyService.UpdatePropertyAsync(property);

            if (vm.NewStatus == vm.CurrentStatus)
            {
                TempData["Warning"] = "Trạng thái được làm mới.";
            }
            else
            {
                TempData["Success"] = $"Đã cập nhật trạng thái: {vm.NewStatus}";
            }

            return RedirectToAction("Details", new { id = vm.PropertyId });
        }

        /// <summary>
        /// Determines whether [is valid status transition] [the specified status].
        /// </summary>
        /// <param name="newStatus">The new status.</param>
        /// <returns>
        ///   <c>true</c> if [is valid status transition] [the specified status]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidStatusTransition(string newStatus)
        {
            string[] validStatuses = { "Available", "Rented", "Maintenance", "Unavailable" }; // Available, Rented, Maintenance, Unavailable
            if (!validStatuses.Contains(newStatus))
            {
                return false;
            }
            return true; // For simplicity, allow any transition between valid statuses regardless of current status
        }
    }
}
