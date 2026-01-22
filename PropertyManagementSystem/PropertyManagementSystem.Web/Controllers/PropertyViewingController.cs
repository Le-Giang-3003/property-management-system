using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Schedule;
using PropertyManagementSystem.BLL.Services.Interface;
using System.Security.Claims;

namespace PropertyManagementSystem.Web.Controllers
{
    public class PropertyViewingController : Controller
    {
        private readonly IPropertyViewingService _viewingService;

        public PropertyViewingController(IPropertyViewingService viewingService)
        {
            _viewingService = viewingService;
        }

        #region Helper Methods

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private string? GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }

        #endregion

        #region Tenant/Guest Actions

        // GET: Trang request viewing cho property
        [HttpGet]
        public IActionResult Request(int propertyId)
        {
            var model = new CreateViewingRequestDto
            {
                PropertyId = propertyId,
                PreferredDate1 = DateTime.Now.AddDays(1).Date.AddHours(10) // Default 10:00 AM ngày mai
            };
            return View(model);
        }

        // POST: Submit viewing request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Request(CreateViewingRequestDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = GetCurrentUserId();
            var (success, message, viewingId) = await _viewingService.CreateViewingRequestAsync(model, userId);

            if (success)
            {
                TempData["SuccessMessage"] = message;
                if (userId.HasValue)
                    return RedirectToAction(nameof(MyViewings));
                else
                    return RedirectToAction(nameof(RequestConfirmation), new { id = viewingId });
            }

            TempData["ErrorMessage"] = message;
            return View(model);
        }

        // GET: Confirmation page cho guest
        [HttpGet]
        public async Task<IActionResult> RequestConfirmation(int id)
        {
            var viewing = await _viewingService.GetViewingByIdAsync(id);
            if (viewing == null)
                return NotFound();

            return View(viewing);
        }

        // GET: Danh sách viewing của tenant
        [Authorize(Roles = "Tenant")]
        [HttpGet]
        public async Task<IActionResult> MyViewings()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login", "Auth");

            var viewings = await _viewingService.GetViewingsByTenantAsync(userId.Value);
            return View(viewings);
        }

        // POST: Tenant cancel viewing
        [Authorize(Roles = "Tenant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetCurrentUserId();
            var (success, message) = await _viewingService.CancelViewingAsync(id, userId);

            if (success)
                TempData["SuccessMessage"] = message;
            else
                TempData["ErrorMessage"] = message;

            return RedirectToAction(nameof(MyViewings));
        }

        // GET: Form feedback
        [Authorize(Roles = "Tenant")]
        [HttpGet]
        public async Task<IActionResult> Feedback(int id)
        {
            var viewing = await _viewingService.GetViewingByIdAsync(id);
            if (viewing == null)
                return NotFound();

            var model = new ViewingFeedbackRequestDto { ViewingId = id };
            ViewBag.Viewing = viewing;
            return View(model);
        }

        // POST: Submit feedback
        [Authorize(Roles = "Tenant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Feedback(ViewingFeedbackRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                var viewing = await _viewingService.GetViewingByIdAsync(model.ViewingId);
                ViewBag.Viewing = viewing;
                return View(model);
            }

            var userId = GetCurrentUserId();
            var (success, message) = await _viewingService.SubmitFeedbackAsync(model, userId);

            if (success)
                TempData["SuccessMessage"] = message;
            else
                TempData["ErrorMessage"] = message;

            return RedirectToAction(nameof(MyViewings));
        }

        #endregion

        #region Landlord Actions

        // GET: Danh sách tất cả viewing requests cho landlord
        [Authorize(Roles = "Landlord")]
        [HttpGet]
        public async Task<IActionResult> ManageViewings()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login", "Auth");

            var viewings = await _viewingService.GetViewingsByLandlordAsync(userId.Value);
            return View(viewings);
        }

        // GET: Danh sách pending requests
        [Authorize(Roles = "Landlord")]
        [HttpGet]
        public async Task<IActionResult> PendingRequests()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login", "Auth");

            var viewings = await _viewingService.GetPendingViewingsAsync(userId.Value);
            return View(viewings);
        }

        // GET: Chi tiết viewing
        [Authorize(Roles = "Landlord,Tenant,Admin")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var viewing = await _viewingService.GetViewingByIdAsync(id);
            if (viewing == null)
                return NotFound();

            return View(viewing);
        }

        // GET: Form confirm viewing
        [Authorize(Roles = "Landlord")]
        [HttpGet]
        public async Task<IActionResult> Confirm(int id)
        {
            var viewing = await _viewingService.GetViewingByIdAsync(id);
            if (viewing == null)
                return NotFound();

            var model = new ConfirmViewingRequestDto
            {
                ViewingId = id,
                ScheduledDate = viewing.PreferredDate1 ?? DateTime.Now.AddDays(1)
            };
            ViewBag.Viewing = viewing;
            return View(model);
        }

        // POST: Confirm viewing
        [Authorize(Roles = "Landlord")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(ConfirmViewingRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                var viewing = await _viewingService.GetViewingByIdAsync(model.ViewingId);
                ViewBag.Viewing = viewing;
                return View(model);
            }

            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login", "Auth");

            var (success, message) = await _viewingService.ConfirmViewingAsync(model, userId.Value);

            if (success)
                TempData["SuccessMessage"] = message;
            else
                TempData["ErrorMessage"] = message;

            return RedirectToAction(nameof(ManageViewings));
        }

        // POST: Reject viewing
        [Authorize(Roles = "Landlord")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? reason)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login", "Auth");

            var (success, message) = await _viewingService.RejectViewingAsync(id, reason, userId.Value);

            if (success)
                TempData["SuccessMessage"] = message;
            else
                TempData["ErrorMessage"] = message;

            return RedirectToAction(nameof(PendingRequests));
        }

        // POST: Mark as completed
        [Authorize(Roles = "Landlord")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login", "Auth");

            var (success, message) = await _viewingService.CompleteViewingAsync(id, userId.Value);

            if (success)
                TempData["SuccessMessage"] = message;
            else
                TempData["ErrorMessage"] = message;

            return RedirectToAction(nameof(ManageViewings));
        }

        #endregion

        #region Shared Actions

        // GET: Upcoming viewings (cả Tenant và Landlord)
        [Authorize(Roles = "Landlord,Tenant")]
        [HttpGet]
        public async Task<IActionResult> Upcoming()
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            if (!userId.HasValue || string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "Auth");

            var viewings = await _viewingService.GetUpcomingViewingsAsync(userId.Value, role);
            return View(viewings);
        }

        // GET: Reschedule form
        [Authorize(Roles = "Landlord,Tenant")]
        [HttpGet]
        public async Task<IActionResult> Reschedule(int id)
        {
            var viewing = await _viewingService.GetViewingByIdAsync(id);
            if (viewing == null)
                return NotFound();

            var model = new RescheduleViewingRequestDto
            {
                ViewingId = id,
                NewScheduledDate = viewing.ScheduledDate ?? DateTime.Now.AddDays(1)
            };
            ViewBag.Viewing = viewing;
            return View(model);
        }

        // POST: Reschedule
        [Authorize(Roles = "Landlord,Tenant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reschedule(RescheduleViewingRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                var viewing = await _viewingService.GetViewingByIdAsync(model.ViewingId);
                ViewBag.Viewing = viewing;
                return View(model);
            }

            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            if (!userId.HasValue || string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "Auth");

            var (success, message) = await _viewingService.RescheduleViewingAsync(model, userId.Value, role);

            if (success)
                TempData["SuccessMessage"] = message;
            else
                TempData["ErrorMessage"] = message;

            if (role == "Landlord")
                return RedirectToAction(nameof(ManageViewings));
            else
                return RedirectToAction(nameof(MyViewings));
        }

        // AJAX: Check time slot availability
        [HttpGet]
        public async Task<IActionResult> CheckAvailability(int propertyId, DateTime scheduledDate, int? excludeViewingId)
        {
            var isAvailable = await _viewingService.CheckTimeSlotAvailabilityAsync(propertyId, scheduledDate, excludeViewingId);
            return Json(new { available = isAvailable });
        }

        #endregion
    }
}
