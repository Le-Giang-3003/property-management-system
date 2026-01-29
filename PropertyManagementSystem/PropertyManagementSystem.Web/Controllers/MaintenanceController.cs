using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Maintenance;
using PropertyManagementSystem.BLL.Services.Interface;
using System.Security.Claims;

namespace PropertyManagementSystem.Web.Controllers
{
    [Authorize]
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly ILeaseService _leaseService;

        public MaintenanceController(IMaintenanceService maintenanceService, ILeaseService leaseService)
        {
            _maintenanceService = maintenanceService;
            _leaseService = leaseService;
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
            
            // Debug: Log if no properties found
            if (properties == null || !properties.Any())
            {
                System.Diagnostics.Debug.WriteLine($"[CreateRequest] UserId: {userId}, Properties count: 0");
                // Try to get leases directly for debugging
                var leases = await _leaseService.GetTenantActivePropertiesAsync(userId);
                System.Diagnostics.Debug.WriteLine($"[CreateRequest] Direct call returned {leases?.Count ?? 0} properties");
            }
            
            ViewBag.Properties = properties;
            return View();
        }

        // POST: /Maintenance/CreateRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest(CreateMaintenanceRequestDto dto)
        {
            var userId = GetCurrentUserId();

            if (!ModelState.IsValid)
            {
                var errors = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = "Vui lòng sửa lỗi: " + errors;
                ViewBag.Properties = await GetUserPropertiesAsync(userId);
                return View(dto);
            }

            try
            {
                var result = await _maintenanceService.CreateRequestAsync(dto, userId);
                TempData["SuccessMessage"] = "Maintenance request created successfully!";
                return RedirectToAction(nameof(TenantIndex));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating request: {ex.Message}";
                // Reload properties for the view
                ViewBag.Properties = await GetUserPropertiesAsync(userId);
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

        // POST: /Maintenance/RejectRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(RejectMaintenanceRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid rejection data.";
                return RedirectToAction(nameof(LandlordIndex));
            }

            try
            {
                var userId = GetCurrentUserId();
                var result = await _maintenanceService.RejectRequestAsync(dto, userId);

                if (result)
                {
                    TempData["SuccessMessage"] = "Request rejected successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to reject request.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error rejecting request: {ex.Message}";
            }

            return RedirectToAction(nameof(LandlordIndex));
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

        // POST: /Maintenance/CloseRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseRequest(int requestId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _maintenanceService.CloseRequestAsync(requestId, userId);

                if (result)
                {
                    TempData["SuccessMessage"] = "Request closed successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to close request.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error closing request: {ex.Message}";
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
            // Remove validation errors for fields that are not required based on status
            if (dto.Status == "Accepted")
            {
                ModelState.Remove(nameof(dto.RejectionReason));
            }
            else if (dto.Status == "Rejected")
            {
                ModelState.Remove(nameof(dto.EstimatedCost));
                ModelState.Remove(nameof(dto.Notes));
            }

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = $"Invalid response data: {errors}";
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
        public async Task<IActionResult> UpdateRequestStatus(int requestId, string status, bool returnToDetails = false)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _maintenanceService.UpdateRequestStatusAsync(requestId, status, userId);
                TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
                    ? "Status updated."
                    : "Unable to update status.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            if (returnToDetails)
                return RedirectToAction(nameof(Details), new { id = requestId });
            return RedirectToAction(nameof(TechnicianIndex));
        }

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
            var result = await _leaseService.GetTenantActivePropertiesAsync(userId);
            Console.WriteLine($"[MaintenanceController.GetUserPropertiesAsync] UserId: {userId}, Properties returned: {result?.Count ?? 0}");
            return result ?? new List<PropertySelectDto>();
        }

        #endregion

        #region Debug Endpoint

        [HttpGet]
        public async Task<IActionResult> DebugProperties()
        {
            var userId = GetCurrentUserId();
            var properties = await GetUserPropertiesAsync(userId);
            var leases = await _leaseService.GetTenantActivePropertiesAsync(userId);
            
            return Json(new 
            { 
                userId, 
                propertiesCount = properties?.Count ?? 0, 
                properties,
                leasesCount = leases?.Count ?? 0,
                leases
            });
        }

        #endregion
    }
}
