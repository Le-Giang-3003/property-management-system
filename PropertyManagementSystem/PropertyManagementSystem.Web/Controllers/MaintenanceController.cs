using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.BLL.DTOs.Maintenance;
using PropertyManagementSystem.BLL.Services.Implementation;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL;
using PropertyManagementSystem.DAL.Data;
using System.Security.Claims;

namespace PropertyManagementSystem.Web.Controllers
{
    [Authorize]
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly AppDbContext _context;



        public MaintenanceController(IMaintenanceService maintenanceService, AppDbContext context)
        {
            _maintenanceService = maintenanceService;
            _context = context;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        private bool IsInRole(string roleName)
        {
            return User.IsInRole(roleName);
        }

        #region Tenant Actions

        // GET: /Maintenance/TenantIndex
        public async Task<IActionResult> TenantIndex()
        {
            var userId = GetCurrentUserId();
            var requests = await _maintenanceService.GetTenantRequestsAsync(userId);
            var stats = await _maintenanceService.GetTenantStatsAsync(userId);

            ViewBag.Stats = stats;
            return View(requests);
        }

        // GET: /Maintenance/CreateRequest
        public async Task<IActionResult> CreateRequest()
        {
            var userId = GetCurrentUserId();
            var properties = await GetUserPropertiesAsync(userId);
            ViewBag.Properties = properties;
            return View();
        }

        // POST: /Maintenance/CreateRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest(CreateMaintenanceRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var userId = GetCurrentUserId();
                var result = await _maintenanceService.CreateRequestAsync(dto, userId);
                TempData["SuccessMessage"] = "Maintenance request created successfully!";
                return RedirectToAction(nameof(TenantIndex));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating request: {ex.Message}";
                return View(dto);
            }
        }

        // POST: /Maintenance/CancelRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRequest(int requestId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _maintenanceService.CancelRequestAsync(requestId, userId);

                if (result)
                {
                    TempData["SuccessMessage"] = "Request cancelled successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to cancel request. It may be in progress or already completed.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error cancelling request: {ex.Message}";
            }

            return RedirectToAction(nameof(TenantIndex));
        }

        // POST: /Maintenance/RateMaintenance
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RateMaintenance(RateMaintenanceDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid rating data.";
                return RedirectToAction(nameof(TenantIndex));
            }

            try
            {
                var userId = GetCurrentUserId();
                var result = await _maintenanceService.RateMaintenanceAsync(dto, userId);

                if (result)
                {
                    TempData["SuccessMessage"] = "Rating submitted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to rate this request.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error submitting rating: {ex.Message}";
            }

            return RedirectToAction(nameof(TenantIndex));
        }

        #endregion

        #region Landlord Actions

        // GET: /Maintenance/LandlordIndex
        public async Task<IActionResult> LandlordIndex()
        {
            var userId = GetCurrentUserId();
            var requests = await _maintenanceService.GetLandlordRequestsAsync(userId);
            var stats = await _maintenanceService.GetLandlordStatsAsync(userId);
            var technicians = await _maintenanceService.GetAvailableTechniciansAsync();

            ViewBag.Stats = stats;
            ViewBag.Technicians = technicians;
            return View(requests);
        }

        // POST: /Maintenance/AssignTechnician
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTechnician(AssignTechnicianDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid assignment data.";
                return RedirectToAction(nameof(LandlordIndex));
            }

            try
            {
                var userId = GetCurrentUserId();
                var result = await _maintenanceService.AssignTechnicianAsync(dto, userId);

                if (result)
                {
                    TempData["SuccessMessage"] = "Technician assigned successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to assign technician.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error assigning technician: {ex.Message}";
            }

            return RedirectToAction(nameof(LandlordIndex));
        }

        // POST: /Maintenance/ConfirmCompletion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmCompletion(int requestId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _maintenanceService.ConfirmCompletionAsync(requestId, userId);

                if (result)
                {
                    TempData["SuccessMessage"] = "Maintenance completion confirmed!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to confirm completion.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error confirming completion: {ex.Message}";
            }

            return RedirectToAction(nameof(LandlordIndex));
        }

        #endregion

        #region Technician Actions

        // GET: /Maintenance/TechnicianIndex
        public async Task<IActionResult> TechnicianIndex()
        {
            var userId = GetCurrentUserId();
            var requests = await _maintenanceService.GetTechnicianRequestsAsync(userId);
            var stats = await _maintenanceService.GetTechnicianStatsAsync(userId);

            ViewBag.Stats = stats;
            return View(requests);
        }

        // POST: /Maintenance/RespondToAssignment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RespondToAssignment(TechnicianResponseDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid response data.";
                return RedirectToAction(nameof(TechnicianIndex));
            }

            try
            {
                var userId = GetCurrentUserId();
                var result = await _maintenanceService.RespondToAssignmentAsync(dto, userId);

                if (result)
                {
                    TempData["SuccessMessage"] = dto.Status == "Accepted"
                        ? "Assignment accepted successfully!"
                        : "Assignment rejected.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to respond to assignment.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error responding to assignment: {ex.Message}";
            }

            return RedirectToAction(nameof(TechnicianIndex));
        }

        // POST: /Maintenance/CompleteMaintenance
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteMaintenance(CompleteMaintenanceDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid completion data.";
                return RedirectToAction(nameof(TechnicianIndex));
            }

            try
            {
                var userId = GetCurrentUserId();
                var result = await _maintenanceService.CompleteMaintenanceAsync(dto, userId);

                if (result)
                {
                    TempData["SuccessMessage"] = "Maintenance marked as completed!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to complete maintenance.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error completing maintenance: {ex.Message}";
            }

            return RedirectToAction(nameof(TechnicianIndex));
        }

        #endregion

        #region Common Actions

        // GET: /Maintenance/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var request = await _maintenanceService.GetRequestByIdAsync(id);
                if (request == null)
                {
                    TempData["ErrorMessage"] = "Request not found.";
                    return RedirectToAction(nameof(TenantIndex));
                }

                // Load technicians for landlord view
                if (IsInRole("Landlord") || IsInRole("Member"))
                {
                    var technicians = await _maintenanceService.GetAvailableTechniciansAsync();
                    ViewBag.Technicians = technicians;
                }

                return View(request);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading request details: {ex.Message}";
                return RedirectToAction(nameof(TenantIndex));
            }
        }

        // POST: /Maintenance/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int requestId, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                TempData["ErrorMessage"] = "Comment cannot be empty.";
                return RedirectToAction(nameof(Details), new { id = requestId });
            }

            try
            {
                var userId = GetCurrentUserId();
                var result = await _maintenanceService.AddCommentAsync(requestId, userId, comment);

                if (result)
                {
                    TempData["SuccessMessage"] = "Comment added successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to add comment.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error adding comment: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id = requestId });
        }

        // API endpoint for filtering
        [HttpGet]
        public async Task<IActionResult> GetRequestsByStatus(string status)
        {
            try
            {
                var userId = GetCurrentUserId();
                List<MaintenanceRequestDto> requests;

                if (IsInRole("Landlord"))
                {
                    requests = await _maintenanceService.GetLandlordRequestsAsync(userId, status);
                }
                else if (IsInRole("Technician"))
                {
                    requests = await _maintenanceService.GetTechnicianRequestsAsync(userId, status);
                }
                else
                {
                    requests = await _maintenanceService.GetTenantRequestsAsync(userId, status);
                }

                return Json(requests);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        #endregion

        #region Helper Methods

        private async Task<List<PropertySelectDto>> GetUserPropertiesAsync(int userId)
        {
            var properties = await _context.Leases
                .Include(l => l.Property)
                .Where(l => l.TenantId == userId && l.Status == "Active")
                .Select(l => new PropertySelectDto
                {
                    PropertyId = l.PropertyId,
                    Name = l.Property.Name,
                    Address = l.Property.Address
                })
                .ToListAsync();

            return properties;
        }

        #endregion
    }
}
