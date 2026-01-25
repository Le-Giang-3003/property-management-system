using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.Web.ViewModels.Dashboard;
using System.Security.Claims;

namespace PropertyManagementSystem.Web.Controllers
{
    /// <summary>
    /// Controller for routing to the dashboard views base on role.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class DashboardController : Controller
    {
        /// <summary>
        /// The dashboard service
        /// </summary>
        private readonly IDashboardService _dashboardService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardController"/> class.
        /// </summary>
        /// <param name="dashboardService">The dashboard service.</param>
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        /// <summary>
        /// Landlords the index.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> LandlordIndex()
        {
            Console.WriteLine("=== LandlordIndex Action Called ===");
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int landlordId))
            {
                TempData["Error"] = "Không tìm thấy thông tin người dùng";
                return RedirectToAction("Login", "Auth");
            }
            Console.WriteLine($"✅ Landlord ID: {landlordId}");
            try
            {
                Console.WriteLine("Calling GetLandlordDashboardAsync...");
                var dto = await _dashboardService.GetLandlordDashboardAsync(landlordId);
                Console.WriteLine("✅ DTO received successfully");
                // Map DTO to ViewModel
                var viewModel = new LandlordDashboardViewModel
                {
                    TotalProperties = dto.TotalProperties,
                    AvailableProperties = dto.AvailableProperties,
                    RentedProperties = dto.RentedProperties,
                    MaintenanceProperties = dto.MaintenanceProperties,
                    TotalMonthlyRevenue = dto.TotalMonthlyRevenue,
                    OccupancyRate = dto.OccupancyRate,
                    PendingMaintenanceRequests = dto.PendingMaintenanceRequests,
                    PendingApplicationsCount = dto.PendingApplicationsCount,
                    RecentProperties = dto.RecentProperties,
                    RecentApplications = dto.RecentApplications,
                    RecentMaintenanceRequests = dto.RecentMaintenanceRequests,
                    ExpiringContracts = dto.ExpiringContracts
                };
                Console.WriteLine("✅ ViewModel mapped successfully");
                Console.WriteLine("Returning View...");
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                TempData["Error"] = "Có lỗi xảy ra khi tải dữ liệu dashboard";
                return RedirectToAction("Index", "Home");
            }
        }
        /// <summary>
        /// Indexes this instance.
        /// Tenant Dashboard View.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
