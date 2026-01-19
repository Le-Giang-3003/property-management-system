using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.Web.ViewModels;

namespace PropertyManagementSystem.Web.Controllers
{
    public class PropertyController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IUserService _userService;

        public PropertyController(IPropertyService propertyService, IUserService userService)
        {
            _propertyService = propertyService;
            _userService = userService;
        }

        // GET: Property/Index
        public async Task<IActionResult> Index(string? city, string? propertyType, decimal? minRent, decimal? maxRent)
        {
            IEnumerable<Property> properties = new List<Property>();

            try
            {
                if (!string.IsNullOrEmpty(city))
                {
                    properties = await _propertyService.SearchPropertiesAsync(city, propertyType, minRent, maxRent);
                    ViewBag.City = city;
                    ViewBag.PropertyType = propertyType;
                    ViewBag.MinRent = minRent;
                    ViewBag.MaxRent = maxRent;
                }
                else
                {
                    properties = await _propertyService.GetAllPropertiesAsync();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading properties: " + ex.Message;
                properties = new List<Property>();
            }

            ViewBag.PropertyTypes = GetPropertyTypesSelectList();

            // ✓ Sửa: Chỉ định view "PropertyManagement"
            return View("PropertyManagement", properties ?? new List<Property>());
        }

        // GET: Property/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            // ❌ SAI: Details không nên dùng view PropertyManagement
            // ✓ ĐÚNG: Tạo view Details.cshtml riêng
            return View("PropertyDetail", property);
        }

        private async Task<List<SelectListItem>> GetLandlordsSelectList()
        {
            var landlords = await _userService.GetUsersByRoleAsync("Landlord");
            return landlords.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.FullName
            }).ToList();
        }

        private List<SelectListItem> GetPropertyTypesSelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Select Type --" },
                new SelectListItem { Value = "Apartment", Text = "Apartment" },
                new SelectListItem { Value = "House", Text = "House" },
                new SelectListItem { Value = "Villa", Text = "Villa" },
                new SelectListItem { Value = "Office", Text = "Office" }
            };
        }

        // GET: Property/Create
        public async Task<IActionResult> Create()
        {
            var vm = new CreatePropertyViewModel
            {
                AvailableLandlords = await GetLandlordsSelectList(),
                PropertyTypes = GetPropertyTypesSelectList()
            };
            return View("PropertyCreate",vm);
        }

        // POST: Property/Create
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
                        Title = vm.Title,
                        Description = vm.Description,
                        Address = vm.Address,
                        City = vm.City,
                        District = vm.District,
                        PropertyType = vm.PropertyType,
                        BaseRentPrice = vm.BaseRentPrice,
                        Area = vm.Area,
                        LandlordId = vm.LandlordId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Status = "Available"
                    };

                    await _propertyService.AddPropertyAsync(property);
                    TempData["Success"] = "Property created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            vm.AvailableLandlords = await GetLandlordsSelectList();
            vm.PropertyTypes = GetPropertyTypesSelectList();
            return View("PropertyCreate", vm);
        }

        // GET: Property/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return View("PropertyEdit", property);
        }

        // POST: Property/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Property property)
        {
            if (id != property.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    property.UpdatedAt = DateTime.UtcNow;  // Set UpdatedAt
                    await _propertyService.UpdatePropertyAsync(property);
                    TempData["Success"] = "Property updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View("PropertyEdit",property);
        }


        // GET: Property/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return View("PropertyDelete",property);
        }

        // POST: Property/Delete/{id}
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
    }
}
