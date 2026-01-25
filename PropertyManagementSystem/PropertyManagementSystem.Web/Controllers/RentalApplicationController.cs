using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManagementSystem.BLL.DTOs.Application;
using PropertyManagementSystem.BLL.Services.Implementation;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.Web.ViewModels.Application;
using System.Security.Claims;

namespace PropertyManagementSystem.Web.Controllers
{
    //[Authorize]
    public class RentalApplicationController : Controller
    {
        private readonly IRentalApplicationService _applicationService;
        private readonly IPropertyService _propertyService;
        private readonly ILeaseService _leaseService;

        public RentalApplicationController(
                IRentalApplicationService applicationService,
                IPropertyService propertyService,
                ILeaseService leaseService) 
        {
            _applicationService = applicationService;
            _propertyService = propertyService;
            _leaseService = leaseService; 
        }

        // GET: /RentalApplication/Create
        [HttpGet]
        public async Task<IActionResult> Create(int? propertyId)
        {
            // ✅ LẤY USER ID HIỆN TẠI
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var availableProperties = await _propertyService.GetAllPropertiesAsync();

            // ✅ FILTER: Bỏ property của chính user + chỉ lấy Available
            var propertyList = availableProperties
                .Where(p => p.Status == "Available" && p.LandlordId != userId) // ← THÊM FILTER
                .Select(p => new SelectListItem
                {
                    Value = p.PropertyId.ToString(),
                    Text = $"{p.Name} - {p.Address} ({p.RentAmount:N0} VNĐ/tháng)"
                })
                .ToList();

            if (!propertyList.Any())
            {
                TempData["Error"] = "There are no properties available for rent at the moment";
                return RedirectToAction("SearchProperties", "Property");
            }

            var viewModel = new CreateRentalApplicationViewModel
            {
                AvailableProperties = propertyList,
                DesiredMoveInDate = DateTime.Now.AddDays(30),
                NumberOfOccupants = 1
            };

            // ✅ NẾU CÓ PROPERTY ID, VALIDATE KHÔNG PHẢI CỦA MÌNH
            if (propertyId.HasValue && propertyId.Value > 0)
            {
                var property = await _propertyService.GetPropertyByIdAsync(propertyId.Value);

                // ✅ KIỂM TRA: Property phải Available VÀ không phải của mình
                if (property != null && property.Status == "Available" && property.LandlordId != userId)
                {
                    viewModel.PropertyId = propertyId.Value;
                    viewModel.PropertyName = property.Name;
                    viewModel.PropertyAddress = property.Address;
                    viewModel.MonthlyRent = property.RentAmount;
                }
                else if (property?.LandlordId == userId)
                {
                    TempData["Warning"] = "You cannot submit a rental application for your own property";
                }
            }

            return View("ApplicationCreate", viewModel);
        }

        // POST: /RentalApplication/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRentalApplicationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var availableProperties = await _propertyService.GetAllPropertiesAsync();
                model.AvailableProperties = availableProperties
                    .Where(p => p.Status == "Available")
                    .Select(p => new SelectListItem
                    {
                        Value = p.PropertyId.ToString(),
                        Text = $"{p.Name} - {p.Address} ({p.RentAmount:N0} VNĐ/tháng)"
                    });

                return View("ApplicationCreate", model);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var dto = new CreateRentalApplicationDto
            {
                PropertyId = model.PropertyId,
                EmploymentStatus = model.EmploymentStatus,
                Employer = model.Employer,
                MonthlyIncome = model.MonthlyIncome,
                PreviousAddress = model.PreviousAddress,
                PreviousLandlord = model.PreviousLandlord,
                PreviousLandlordPhone = model.PreviousLandlordPhone,
                NumberOfOccupants = model.NumberOfOccupants,
                HasPets = model.HasPets,
                PetDetails = model.PetDetails,
                DesiredMoveInDate = model.DesiredMoveInDate,
                AdditionalNotes = model.AdditionalNotes
            };

            var application = await _applicationService.SubmitApplicationAsync(dto, userId);

            if (application == null)
            {
                TempData["Error"] = "Unable to submit rental application. The property may no longer be available.";

                var availableProperties = await _propertyService.GetAllPropertiesAsync();
                model.AvailableProperties = availableProperties
                    .Where(p => p.Status == "Available")
                    .Select(p => new SelectListItem
                    {
                        Value = p.PropertyId.ToString(),
                        Text = $"{p.Name} - {p.Address} ({p.RentAmount:N0} VNĐ/tháng)"
                    });

                return View("ApplicationCreate", model);
            }

            TempData["Success"] = $"Application {application.ApplicationNumber} has been submitted successfully!";
            return RedirectToAction("MyApplications");
        }

        // GET: /RentalApplication/MyApplications
        //[Authorize(Roles = "Tenant,Guest")]
        public async Task<IActionResult> MyApplications()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var applications = await _applicationService.GetApplicationsByApplicantAsync(userId);

            return View("ApplicationMyApplications", applications); // ✅ ĐÚNG TÊN FILE
        }

        // GET: /RentalApplication/Index
        //[Authorize(Roles = "Landlord")]
        public async Task<IActionResult> Index(string? status)
        {
            var applications = await _applicationService.GetAllApplicationsAsync();

            if (!string.IsNullOrEmpty(status))
            {
                applications = applications.Where(a => a.Status == status);
            }

            ViewBag.SelectedStatus = status;
            return View("ApplicationIndex", applications); // ✅ ĐÚNG TÊN FILE
        }
        // GET: /RentalApplication/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var application = await _applicationService.GetApplicationByIdAsync(id);
            if (application == null)
            {
                TempData["Error"] = "Rental application not found";
                return RedirectToAction("Index");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "member")
            {
                if (application.ApplicantId != userId)
                {
                    TempData["Error"] = "You do not have permission to view this application";
                    return RedirectToAction("MyApplications");
                }
            }

            // ✅ THÊM: Kiểm tra xem đã có Lease chưa
            if (application.Status == "Approved")
            {
                var existingLease = await _leaseService.GetLeaseByApplicationIdAsync(id);
                ViewBag.ExistingLease = existingLease;
            }

            return View("ApplicationDetails", application);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var application = await _applicationService.GetApplicationByIdAsync(id);
            if (application == null)
            {
                TempData["Error"] = "Rental application not found";
                return RedirectToAction("Index");
            }

            // ✅ VALIDATE: Only Property Owner can approve
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var property = await _propertyService.GetPropertyByIdAsync(application.PropertyId);

            if (property == null || property.LandlordId != userId)
            {
                TempData["Error"] = "You do not have permission to approve this application";
                return RedirectToAction("Details", new { id });
            }

            var success = await _applicationService.ApproveApplicationAsync(id, userId);

            if (success)
            {
                TempData["Success"] = "Application approved successfully!";
            }
            else
            {
                TempData["Error"] = "Unable to approve this application";
            }

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string rejectionReason)
        {
            if (string.IsNullOrWhiteSpace(rejectionReason))
            {
                TempData["Error"] = "Please enter a rejection reason";
                return RedirectToAction("Details", new { id });
            }

            var application = await _applicationService.GetApplicationByIdAsync(id);
            if (application == null)
            {
                TempData["Error"] = "Rental application not found";
                return RedirectToAction("Index");
            }

            // ✅ VALIDATE: Only Property Owner can reject
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var property = await _propertyService.GetPropertyByIdAsync(application.PropertyId);

            if (property == null || property.LandlordId != userId)
            {
                TempData["Error"] = "You do not have permission to reject this application";
                return RedirectToAction("Details", new { id });
            }

            var success = await _applicationService.RejectApplicationAsync(id, rejectionReason, userId);

            if (success)
            {
                TempData["Success"] = "Application rejected";
            }
            else
            {
                TempData["Error"] = "Unable to reject this application";
            }

            return RedirectToAction("Details", new { id });
        }

        // POST: /RentalApplication/Withdraw/5
        [HttpPost]
        //[Authorize(Roles = "Tenant,Guest")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var withdrawn = await _applicationService.WithdrawApplicationAsync(id, userId);

            if (withdrawn)
            {
                TempData["Success"] = "Application withdrawn successfully";
            }
            else
            {
                TempData["Error"] = "Unable to withdraw application. It may have already been processed.";
            }

            return RedirectToAction("MyApplications");
        }

        // GET: /RentalApplication/ByProperty/5
        //[Authorize(Roles = "Landlord")]
        public async Task<IActionResult> ByProperty(int propertyId)
        {
            var property = await _propertyService.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                TempData["Error"] = "Property not found";
                return RedirectToAction("Index", "Property");
            }

            var applications = await _applicationService.GetApplicationsByPropertyAsync(propertyId);

            ViewBag.PropertyName = property.Name;
            ViewBag.PropertyId = propertyId;

            return View("ApplicationByProperty", applications); // ✅ ĐÚNG TÊN FILE
        }
    }
}
