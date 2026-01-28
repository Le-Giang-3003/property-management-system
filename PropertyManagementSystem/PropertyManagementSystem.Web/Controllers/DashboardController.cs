using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IMaintenanceService _maintenanceService;

        public DashboardController(IDashboardService dashboardService, IMaintenanceService maintenanceService)
        {
            _dashboardService = dashboardService;
            _maintenanceService = maintenanceService;
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
                TempData["Error"] = "User information not found";
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
                TempData["Error"] = "An error occurred while loading dashboard data";
                return RedirectToAction("Index", "Home");
            }
        }
        /// <summary>
        /// Indexes this instance.
        /// Tenant Dashboard View.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int tenantId))
            {
                TempData["Error"] = "User information not found";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var dto = await _dashboardService.GetTenantDashboardAsync(tenantId);

                var viewModel = new TenantDashboardViewModel
                {
                    ActiveLeasesCount = dto.ActiveLeasesCount,
                    PendingPaymentsCount = dto.PendingPaymentsCount,
                    OpenMaintenanceCount = dto.OpenMaintenanceCount,
                    SavedPropertiesCount = dto.SavedPropertiesCount,
                    UpcomingPayments = dto.UpcomingPayments,
                    ActiveLeases = dto.ActiveLeases,
                    RecentActivities = dto.RecentActivities
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Tenant Dashboard ERROR: {ex.Message}");
                TempData["Error"] = "Error loading dashboard data";
                // Return empty view model to avoid null reference
                return View(new TenantDashboardViewModel());
            }
        }

        /// <summary>
        /// Technician Dashboard View.
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "TechnicianOnly")]
        public async Task<IActionResult> TechnicianIndex()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int technicianId))
            {
                TempData["Error"] = "User information not found";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var requests = await _maintenanceService.GetTechnicianRequestsAsync(technicianId);
                var stats = await _maintenanceService.GetTechnicianStatsAsync(technicianId);

                var viewModel = new TechnicianDashboardViewModel
                {
                    TotalAssignments = stats.TotalRequests,
                    PendingResponse = stats.PendingRequests,
                    InProgress = stats.InProgressRequests,
                    Completed = stats.CompletedRequests,
                    RecentRequests = requests.Take(5).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Technician Dashboard ERROR: {ex.Message}");
                TempData["Error"] = "Error loading dashboard data";
                return View(new TechnicianDashboardViewModel());
            }
        }
    }
}
