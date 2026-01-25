using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Lease;
using PropertyManagementSystem.BLL.Services.Implementation;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.Web.Helpers;
using PropertyManagementSystem.Web.ViewModels.Lease;
using System.Security.Claims;

namespace PropertyManagementSystem.Web.Controllers
{
    [Authorize]
    public class LeaseController : Controller
    {
        private readonly ILeaseService _leaseService;
        private readonly IRentalApplicationService _applicationService;
        private readonly IPropertyService _propertyService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IPdfService _pdfService;
        public LeaseController(
            ILeaseService leaseService,
            IRentalApplicationService applicationService,
            IPropertyService propertyService,
            IUserService userService,
            IEmailService emailService,
            IPdfService pdfService)
        {
            _leaseService = leaseService;
            _applicationService = applicationService;
            _propertyService = propertyService;
            _userService = userService;
            _emailService = emailService;
            _pdfService = pdfService;
        }

        // GET: /Lease/Index
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Lấy tất cả lease mà user là tenant HOẶC landlord
            var allLeases = await _leaseService.GetAllLeasesAsync();
            var leases = allLeases.Where(l =>
                l.TenantId == userId ||
                l.Property?.LandlordId == userId
            ).ToList();

            ViewBag.UserId = userId;
            return View(leases);
        }

        // GET: /Lease/MyLeases (Hợp đồng tôi thuê)
        public async Task<IActionResult> MyLeases()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var leases = await _leaseService.GetLeasesByTenantIdAsync(userId);
            return View(leases);
        }

        // GET: /Lease/MyProperties (Hợp đồng BĐS tôi cho thuê)
        public async Task<IActionResult> MyProperties()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var leases = await _leaseService.GetLeasesByLandlordIdAsync(userId);
            return View(leases);
        }

        // GET: /Lease/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var lease = await _leaseService.GetLeaseByIdAsync(id);
            if (lease == null)
            {
                TempData["Error"] = "Không tìm thấy hợp đồng";
                return RedirectToAction("Index");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Kiểm tra quyền truy cập
            var property = await _propertyService.GetPropertyByIdAsync(lease.PropertyId);
            var isLandlord = property.LandlordId == userId;
            var isTenant = lease.TenantId == userId;

            if (!isLandlord && !isTenant)
            {
                TempData["Error"] = "Bạn không có quyền xem hợp đồng này";
                return RedirectToAction("Index");
            }

            ViewBag.IsLandlord = isLandlord;
            ViewBag.IsTenant = isTenant;

            // ✅ Kiểm tra quyền ký
            ViewBag.CanSign = await _leaseService.CanUserSignAsync(id, userId);

            // ✅ Lấy danh sách chữ ký
            var signatures = await _leaseService.GetLeaseSignaturesAsync(id);
            ViewBag.Signatures = signatures;

            // ✅ Kiểm tra từng bên đã ký chưa
            ViewBag.HasLandlordSigned = signatures.Any(s => s.SignerRole == "Landlord");
            ViewBag.HasTenantSigned = signatures.Any(s => s.SignerRole == "Tenant");

            return View(lease);
        }


        // GET: /Lease/Create?applicationId=5
        [HttpGet]
        public async Task<IActionResult> Create(int applicationId)
        {
            var application = await _applicationService.GetApplicationByIdAsync(applicationId);

            if (application == null || application.Status != "Approved")
            {
                TempData["Error"] = "Đơn xin thuê không hợp lệ hoặc chưa được duyệt";
                return RedirectToAction("Index", "RentalApplication");
            }

            // Kiểm tra quyền: Chỉ landlord mới tạo được
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (application.Property?.LandlordId != userId)
            {
                TempData["Error"] = "Bạn không có quyền tạo hợp đồng cho đơn này";
                return RedirectToAction("Index", "RentalApplication");
            }

            // Kiểm tra đã tạo Lease chưa
            var canCreate = await _leaseService.CanCreateLeaseFromApplication(applicationId);
            if (!canCreate)
            {
                TempData["Error"] = "Hợp đồng cho đơn này đã được tạo rồi";
                return RedirectToAction("Details", "RentalApplication", new { id = applicationId });
            }

            // Tính PaymentDueDay
            int startDay = application.DesiredMoveInDate.Day;
            int paymentDueDay = startDay >= 29 ? 28 : startDay;

            var viewModel = new CreateLeaseViewModel
            {
                ApplicationId = applicationId,
                ApplicationNumber = application.ApplicationNumber,
                PropertyName = application.Property?.Name,
                PropertyAddress = application.Property?.Address,
                TenantName = application.Applicant?.FullName,
                TenantEmail = application.Applicant?.Email,
                TenantPhone = application.Applicant?.PhoneNumber,
                StartDate = application.DesiredMoveInDate,
                OriginalStartDay = startDay,
                LeaseDurationMonths = 12,
                EndDate = application.DesiredMoveInDate.AddMonths(12),
                PaymentDueDay = paymentDueDay,
                MonthlyRent = application.Property?.RentAmount ?? 0,
                SecurityDeposit = (application.Property?.RentAmount ?? 0) * 2
            };

            return View(viewModel);
        }

        // POST: /Lease/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLeaseViewModel model)
        {
            // ✅ LOG 1: Kiểm tra Model nhận được gì
            Console.WriteLine("=== [POST] CREATE LEASE ===");
            Console.WriteLine($"ApplicationId: {model.ApplicationId}");
            Console.WriteLine($"MonthlyRent: {model.MonthlyRent}");
            Console.WriteLine($"SecurityDeposit: {model.SecurityDeposit}");
            Console.WriteLine($"LeaseDurationMonths: {model.LeaseDurationMonths}");
            Console.WriteLine($"StartDate: {model.StartDate}");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                // ✅ LOG 2: In ra tất cả lỗi validation
                Console.WriteLine("❌ ModelState KHÔNG HỢP LỆ:");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    if (errors.Count > 0)
                    {
                        foreach (var error in errors)
                        {
                            Console.WriteLine($"   - {key}: {error.ErrorMessage}");
                        }
                    }
                }

                // Reload data
                var app = await _applicationService.GetApplicationByIdAsync(model.ApplicationId);
                model.ApplicationNumber = app.ApplicationNumber;
                model.PropertyName = app.Property?.Name;
                model.PropertyAddress = app.Property?.Address;
                model.TenantName = app.Applicant?.FullName;
                model.TenantEmail = app.Applicant?.Email;
                model.TenantPhone = app.Applicant?.PhoneNumber;

                return View(model);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Console.WriteLine($"UserId: {userId}");

            var dto = new CreateLeaseDto
            {
                ApplicationId = model.ApplicationId,
                LeaseDurationMonths = model.LeaseDurationMonths,
                MonthlyRent = model.MonthlyRent,
                SecurityDeposit = model.SecurityDeposit,
                Terms = string.IsNullOrWhiteSpace(model.Terms) ? null : model.Terms.Trim(),
                SpecialConditions = string.IsNullOrWhiteSpace(model.SpecialConditions) ? null : model.SpecialConditions.Trim(),
                AutoRenew = model.AutoRenew
            };

            Console.WriteLine("✅ Calling LeaseService.CreateLeaseFromApplicationAsync...");

            try
            {
                var lease = await _leaseService.CreateLeaseFromApplicationAsync(dto, userId);

                if (lease == null)
                {
                    Console.WriteLine("❌ Service trả về NULL");
                    TempData["Error"] = "Không thể tạo hợp đồng. Vui lòng kiểm tra lại.";

                    // Reload data để hiển thị form lại
                    var app = await _applicationService.GetApplicationByIdAsync(model.ApplicationId);
                    model.ApplicationNumber = app.ApplicationNumber;
                    model.PropertyName = app.Property?.Name;
                    model.PropertyAddress = app.Property?.Address;
                    model.TenantName = app.Applicant?.FullName;
                    model.TenantEmail = app.Applicant?.Email;
                    model.TenantPhone = app.Applicant?.PhoneNumber;

                    return View(model);
                }

                Console.WriteLine($"✅ Lease created successfully: {lease.LeaseNumber}");
                TempData["Success"] = $"Hợp đồng {lease.LeaseNumber} đã được tạo thành công!";
                return RedirectToAction("Details", new { id = lease.LeaseId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ EXCEPTION: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                TempData["Error"] = $"Lỗi: {ex.Message}";

                // Reload data
                var app = await _applicationService.GetApplicationByIdAsync(model.ApplicationId);
                model.ApplicationNumber = app.ApplicationNumber;
                model.PropertyName = app.Property?.Name;
                model.PropertyAddress = app.Property?.Address;
                model.TenantName = app.Applicant?.FullName;
                model.TenantEmail = app.Applicant?.Email;
                model.TenantPhone = app.Applicant?.PhoneNumber;

                return View(model);
            }
        }

        // GET: /Lease/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var lease = await _leaseService.GetLeaseByIdAsync(id);
            if (lease == null)
            {
                TempData["Error"] = "Không tìm thấy hợp đồng";
                return RedirectToAction("Index");
            }

            // Chỉ cho phép edit khi Status = Draft
            if (lease.Status != "Draft")
            {
                TempData["Error"] = "Chỉ có thể chỉnh sửa hợp đồng ở trạng thái Nháp";
                return RedirectToAction("Details", new { id });
            }

            // ✅ THÊM: Kiểm tra đã có ai ký chưa
            var isFullySigned = await _leaseService.IsLeaseFullySignedAsync(id);
            if (isFullySigned)
            {
                TempData["Error"] = "Không thể chỉnh sửa hợp đồng đã được ký";
                return RedirectToAction("Details", new { id });
            }

            // ✅ THÊM: Kiểm tra có bất kỳ chữ ký nào chưa
            var signatures = await _leaseService.GetLeaseSignaturesAsync(id);
            if (signatures.Any())
            {
                TempData["Error"] = "Không thể chỉnh sửa hợp đồng đã có người ký. Vui lòng hủy và tạo hợp đồng mới.";
                return RedirectToAction("Details", new { id });
            }

            // Kiểm tra quyền
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (lease.Property?.LandlordId != userId)
            {
                TempData["Error"] = "Bạn không có quyền chỉnh sửa hợp đồng này";
                return RedirectToAction("Details", new { id });
            }

            // Tính số tháng
            int months = ((lease.EndDate.Year - lease.StartDate.Year) * 12) +
                         lease.EndDate.Month - lease.StartDate.Month;

            var viewModel = new UpdateLeaseViewModel
            {
                LeaseId = lease.LeaseId,
                LeaseNumber = lease.LeaseNumber,
                StartDate = lease.StartDate,
                OriginalStartDay = lease.StartDate.Day,
                LeaseDurationMonths = months,
                EndDate = lease.EndDate,
                PaymentDueDay = lease.PaymentDueDay,
                MonthlyRent = lease.MonthlyRent,
                SecurityDeposit = lease.SecurityDeposit,
                Terms = lease.Terms,
                SpecialConditions = lease.SpecialConditions,
                AutoRenew = lease.AutoRenew,
                PropertyName = lease.Property?.Name,
                TenantName = lease.Tenant?.FullName,
                Status = lease.Status
            };

            return View(viewModel);
        }

        // POST: /Lease/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateLeaseViewModel model)
        {
            ModelState.Remove("LeaseNumber");
            ModelState.Remove("PropertyName");
            ModelState.Remove("TenantName");
            ModelState.Remove("Status");

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // ✅ THÊM: Kiểm tra lại trước khi update
            var lease = await _leaseService.GetLeaseByIdAsync(model.LeaseId);
            if (lease == null)
            {
                TempData["Error"] = "Không tìm thấy hợp đồng";
                return RedirectToAction("Index");
            }

            if (lease.Status != "Draft")
            {
                TempData["Error"] = "Chỉ có thể chỉnh sửa hợp đồng ở trạng thái Nháp";
                return RedirectToAction("Details", new { id = model.LeaseId });
            }

            // ✅ Kiểm tra có chữ ký nào chưa
            var signatures = await _leaseService.GetLeaseSignaturesAsync(model.LeaseId);
            if (signatures.Any())
            {
                TempData["Error"] = "Không thể chỉnh sửa hợp đồng đã có người ký";
                return RedirectToAction("Details", new { id = model.LeaseId });
            }
            var dto = new UpdateLeaseDto
            {
                LeaseId = model.LeaseId,
                StartDate = model.StartDate,
                LeaseDurationMonths = model.LeaseDurationMonths,
                MonthlyRent = model.MonthlyRent,
                SecurityDeposit = model.SecurityDeposit,
                Terms = string.IsNullOrWhiteSpace(model.Terms) ? null : model.Terms.Trim(),
                SpecialConditions = string.IsNullOrWhiteSpace(model.SpecialConditions) ? null : model.SpecialConditions.Trim(),
                AutoRenew = model.AutoRenew
            };

            var success = await _leaseService.UpdateLeaseAsync(dto);

            if (success)
            {
                TempData["Success"] = "Cập nhật hợp đồng thành công!";
                return RedirectToAction("Details", new { id = model.LeaseId });
            }
            else
            {
                TempData["Error"] = "Không thể cập nhật hợp đồng";
                return View(model);
            }
        }

        // GET: /Lease/History?propertyId=5
        public async Task<IActionResult> History(int propertyId)
        {
            var property = await _propertyService.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                TempData["Error"] = "Không tìm thấy bất động sản";
                return RedirectToAction("Index", "Property");
            }

            // Kiểm tra quyền
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (property.LandlordId != userId)
            {
                TempData["Error"] = "Bạn không có quyền xem lịch sử hợp đồng này";
                return RedirectToAction("Index", "Property");
            }

            var leases = await _leaseService.GetLeaseHistoryByPropertyIdAsync(propertyId);
            ViewBag.PropertyId = propertyId;
            ViewBag.PropertyName = property.Name;
            return View(leases);
        }
        

        [HttpGet]
        [Route("{id}/Signatures")]
        public async Task<IActionResult> GetSignatures(int id)
        {
            try
            {
                var signatures = await _leaseService.GetLeaseSignaturesAsync(id);
                return Ok(signatures);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("{id}/CanSign")]
        public async Task<IActionResult> CanUserSign(int id, [FromQuery] int userId)
        {
            try
            {
                var canSign = await _leaseService.CanUserSignAsync(id, userId);
                var isFullySigned = await _leaseService.IsLeaseFullySignedAsync(id);

                return Ok(new
                {
                    canSign = canSign,
                    isFullySigned = isFullySigned
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Sign(int id)
        {
            var lease = await _leaseService.GetLeaseByIdAsync(id);
            if (lease == null)
            {
                TempData["Error"] = "Không tìm thấy hợp đồng";
                return RedirectToAction("Index");
            }

            // Kiểm tra status
            if (lease.Status != "Draft")
            {
                TempData["Error"] = "Chỉ có thể ký hợp đồng ở trạng thái Nháp";
                return RedirectToAction("Details", new { id });
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Kiểm tra quyền ký
            var canSign = await _leaseService.CanUserSignAsync(id, userId);
            if (!canSign)
            {
                TempData["Error"] = "Bạn không có quyền ký hợp đồng này hoặc đã ký rồi";
                return RedirectToAction("Details", new { id });
            }

            // Xác định role
            var property = await _propertyService.GetPropertyByIdAsync(lease.PropertyId);
            bool isLandlord = property.LandlordId == userId;
            bool isTenant = lease.TenantId == userId;

            ViewBag.IsLandlord = isLandlord;
            ViewBag.IsTenant = isTenant;
            ViewBag.SignerRole = isLandlord ? "Landlord" : "Tenant";

            // Lấy thông tin signatures đã có
            var signatures = await _leaseService.GetLeaseSignaturesAsync(id);
            ViewBag.Signatures = signatures;

            return View(lease);
        }
        // POST: Sign với OTP
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sign(int id, string? signatureData, string otp)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var lease = await _leaseService.GetLeaseByIdAsync(id);

            if (lease == null)
            {
                TempData["Error"] = "Không tìm thấy hợp đồng";
                return RedirectToAction("Index");
            }

            // ✅ VALIDATE OTP
            var sessionOtp = HttpContext.Session.GetString($"OTP_{id}_{userId}");
            var otpTimeStr = HttpContext.Session.GetString($"OTP_Time_{id}_{userId}");

            if (string.IsNullOrEmpty(sessionOtp) || string.IsNullOrEmpty(otpTimeStr))
            {
                TempData["Error"] = "Vui lòng gửi mã OTP trước khi ký!";
                return RedirectToAction("Details", new { id });
            }

            // Check OTP timeout (5 minutes)
            var otpGeneratedTime = DateTime.Parse(otpTimeStr);
            if ((DateTime.UtcNow - otpGeneratedTime).TotalMinutes > 5)
            {
                HttpContext.Session.Remove($"OTP_{id}_{userId}");
                HttpContext.Session.Remove($"OTP_Time_{id}_{userId}");
                TempData["Error"] = "Mã OTP đã hết hạn (quá 5 phút). Vui lòng gửi lại!";
                return RedirectToAction("Details", new { id });
            }

            // Validate OTP
            if (!OtpHelper.ValidateOtp(sessionOtp, otp))
            {
                TempData["Error"] = "Mã OTP không chính xác! Vui lòng kiểm tra lại.";
                return RedirectToAction("Details", new { id });
            }

            // Clear OTP after successful validation
            HttpContext.Session.Remove($"OTP_{id}_{userId}");
            HttpContext.Session.Remove($"OTP_Time_{id}_{userId}");

            // Xác định role
            var property = await _propertyService.GetPropertyByIdAsync(lease.PropertyId);
            string signerRole = property.LandlordId == userId ? "Landlord" : "Tenant";

            // Lấy IP
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            // ✅ TẠO DTO
            var dto = new SignLeaseDto
            {
                LeaseId = id,
                UserId = userId,
                SignerRole = signerRole,
                SignatureData = signatureData,
                IpAddress = ipAddress
            };

            // ✅ GỌI SERVICE VỚI DTO
            var result = await _leaseService.SignLeaseAsync(dto);

            if (result.Success)
            {
                if (result.IsFullySigned)
                {
                    // ✅ GỬI EMAIL CHO CẢ 2 BÊN
                    try
                    {
                        var landlord = await _userService.GetUserByIdAsync(property.LandlordId);
                        var tenant = await _userService.GetUserByIdAsync(lease.TenantId);

                        var leaseDetailsUrl = Url.Action("Details", "Lease",
                            new { id = lease.LeaseId },
                            protocol: HttpContext.Request.Scheme);

                        // Email cho Landlord
                        var landlordEmailBody = EmailTemplateHelper.CreateLeaseFullySignedEmail(
                            landlord.FullName,
                            lease.LeaseNumber,
                            leaseDetailsUrl
                        );
                        await _emailService.SendEmailAsync(
                            landlord.Email,
                            $"Hợp đồng {lease.LeaseNumber} đã có hiệu lực",
                            landlordEmailBody
                        );

                        // Email cho Tenant
                        var tenantEmailBody = EmailTemplateHelper.CreateLeaseFullySignedEmail(
                            tenant.FullName,
                            lease.LeaseNumber,
                            leaseDetailsUrl
                        );
                        await _emailService.SendEmailAsync(
                            tenant.Email,
                            $"Hợp đồng {lease.LeaseNumber} đã có hiệu lực",
                            tenantEmailBody
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending completion emails: {ex.Message}");
                    }

                    TempData["Success"] = "🎉 Hợp đồng đã được ký đầy đủ và chuyển sang trạng thái Hiệu lực! Email thông báo đã được gửi.";
                }
                else
                {
                    TempData["Success"] = "✅ Bạn đã ký hợp đồng thành công. Đang chờ bên kia ký.";
                }

                return RedirectToAction("Details", new { id });
            }

            TempData["Error"] = result.Message;
            return RedirectToAction("Details", new { id });
        }


        // ✅ POST: Gửi OTP
        [HttpPost]
        public async Task<IActionResult> SendOtp(int leaseId)
        {
            try
            {
                var lease = await _leaseService.GetLeaseByIdAsync(leaseId);
                if (lease == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy hợp đồng" });
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Người dùng";
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userEmail))
                {
                    return Json(new { success = false, message = "Không tìm thấy email của bạn" });
                }

                // Generate OTP
                var otp = OtpHelper.GenerateOtp();

                // Lưu OTP vào Session
                HttpContext.Session.SetString($"OTP_{leaseId}_{userId}", otp);
                HttpContext.Session.SetString($"OTP_Time_{leaseId}_{userId}", DateTime.UtcNow.ToString("o"));

                // Tạo email body
                var emailBody = EmailTemplateHelper.CreateLeaseSigningOtpEmail(userName, lease.LeaseNumber, otp);

                // Gửi email
                await _emailService.SendEmailAsync(
                    userEmail,
                    $"Mã OTP xác nhận ký hợp đồng {lease.LeaseNumber}",
                    emailBody
                );

                return Json(new
                {
                    success = true,
                    message = $"Mã OTP đã được gửi đến {userEmail}"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending OTP: {ex.Message}");
                return Json(new
                {
                    success = false,
                    message = "Không thể gửi email. Vui lòng thử lại sau."
                });
            }
        }

        // GET: /Lease/DownloadPdf/5
        [HttpGet]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var lease = await _leaseService.GetLeaseByIdAsync(id);
            if (lease == null)
            {
                TempData["Error"] = "Không tìm thấy hợp đồng";
                return RedirectToAction("Index");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var property = await _propertyService.GetPropertyByIdAsync(lease.PropertyId);
            var isLandlord = property.LandlordId == userId;
            var isTenant = lease.TenantId == userId;

            if (!isLandlord && !isTenant)
            {
                TempData["Error"] = "Bạn không có quyền tải hợp đồng này";
                return RedirectToAction("Index");
            }

            // ✅ SỬA: Cho phép tải cả Draft và Active
            if (lease.Status != "Draft" && lease.Status != "Active")
            {
                TempData["Error"] = "Không thể tải PDF cho hợp đồng đã hết hạn hoặc bị chấm dứt";
                return RedirectToAction("Details", new { id });
            }

            try
            {
                var pdfBytes = await _pdfService.GenerateLeasePdfAsync(lease);
                var fileName = $"HopDong_{lease.LeaseNumber}_{DateTime.Now:yyyyMMdd}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating PDF: {ex.Message}");
                TempData["Error"] = "Không thể tạo file PDF. Vui lòng thử lại sau.";
                return RedirectToAction("Details", new { id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Terminate(int id)
        {
            var lease = await _leaseService.GetLeaseByIdAsync(id);

            if (lease == null)
                return NotFound();

            if (lease.Status != "Active")
            {
                TempData["Error"] = "Chỉ có thể hủy hợp đồng đang Active";
                return RedirectToAction("Details", new { id });
            }

            var viewModel = new TerminateLeaseViewModel
            {
                LeaseId = lease.LeaseId,
                LeaseNumber = lease.LeaseNumber,
                TenantName = lease.Tenant?.FullName ?? "N/A",
                PropertyName = lease.Property?.Name ?? "N/A",
                PropertyAddress = lease.Property?.Address ?? "N/A",
                StartDate = lease.StartDate,
                EndDate = lease.EndDate,
                MonthlyRent = lease.MonthlyRent,

                // default set hôm nay
                TerminationDate = DateTime.Today
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Terminate(int id, TerminateLeaseViewModel viewModel)
        {
            var lease = await _leaseService.GetLeaseByIdAsync(id);

            if (lease == null)
                return NotFound();

            // Map lại để chạy UI
            viewModel.LeaseId = lease.LeaseId;
            viewModel.LeaseNumber = lease.LeaseNumber;
            viewModel.TenantName = lease.Tenant?.FullName ?? "N/A";
            viewModel.PropertyName = lease.Property?.Name ?? "N/A";
            viewModel.PropertyAddress = lease.Property?.Address ?? "N/A";
            viewModel.StartDate = lease.StartDate;
            viewModel.EndDate = lease.EndDate;
            viewModel.MonthlyRent = lease.MonthlyRent;

            if (!ModelState.IsValid)
                return View(viewModel);

            // Tạo DTO
            var terminateDto = new TerminateLeaseDto
            {
                Reason = viewModel.Reason,
                TerminationDate = viewModel.TerminationDate
            };

            var result = await _leaseService.TerminateLeaseAsync(lease, terminateDto);

            if (result)
            {
                TempData["Success"] = "Hủy hợp đồng thành công";
                return RedirectToAction("Details", new { id });
            }

            TempData["Error"] = "Không thể hủy hợp đồng";
            return View(viewModel);
        }
        // Renew Lease
        [HttpGet]
        public async Task<IActionResult> Renew(int id)
        {
            var lease = await _leaseService.GetLeaseByIdAsync(id);

            if (lease == null)
                return NotFound();

            if (lease.Status != "Active")
            {
                TempData["Error"] = "Chỉ có thể gia hạn hợp đồng đang Active";
                return RedirectToAction("Details", new { id });
            }

            var viewModel = new RenewLeaseViewModel
            {
                LeaseId = lease.LeaseId,
                LeaseNumber = lease.LeaseNumber,
                TenantName = lease.Tenant?.FullName ?? "N/A",
                PropertyName = lease.Property?.Name ?? "N/A",
                PropertyAddress = lease.Property?.Address ?? "N/A",
                StartDate = lease.StartDate,
                EndDate = lease.EndDate,
                CurrentMonthlyRent = lease.MonthlyRent,
                CurrentSecurityDeposit = lease.SecurityDeposit,
                ExtensionMonths = 12,
                AutoRenew = lease.AutoRenew
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renew(int id, RenewLeaseViewModel viewModel)
        {
            var lease = await _leaseService.GetLeaseByIdAsync(id);

            if (lease == null)
                return NotFound();

            viewModel.LeaseId = lease.LeaseId;
            viewModel.LeaseNumber = lease.LeaseNumber;
            viewModel.TenantName = lease.Tenant?.FullName ?? "N/A";
            viewModel.PropertyName = lease.Property?.Name ?? "N/A";
            viewModel.PropertyAddress = lease.Property?.Address ?? "N/A";
            viewModel.StartDate = lease.StartDate;
            viewModel.EndDate = lease.EndDate;
            viewModel.CurrentMonthlyRent = lease.MonthlyRent;
            viewModel.CurrentSecurityDeposit = lease.SecurityDeposit;

            if (!ModelState.IsValid)
                return View(viewModel);

            var renewDto = new RenewLeaseDto
            {
                LeaseId = id,
                ExtensionMonths = viewModel.ExtensionMonths,
                NewMonthlyRent = viewModel.NewMonthlyRent,
                NewSecurityDeposit = viewModel.NewSecurityDeposit,
                AdditionalTerms = viewModel.AdditionalTerms,
                AutoRenew = viewModel.AutoRenew
            };

            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await _leaseService.RenewLeaseAsync(renewDto, userId);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction("Details", new { id = result.NewLeaseId });
            }

            TempData["Error"] = result.Message;
            return View(viewModel);
        }

    }
}
