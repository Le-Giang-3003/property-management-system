using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Payments;
using PropertyManagementSystem.BLL.DTOs.Invoice;
using PropertyManagementSystem.Web.ViewModels.Payment;
using QRCoder;
using System.Drawing.Imaging;
using System.Security.Claims;

namespace PropertyManagementSystem.Web.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        private int? GetTenantId()
        {
            var tenantClaim = User.FindFirst("TenantId");
            if (tenantClaim != null && int.TryParse(tenantClaim.Value, out var tenantId))
            {
                return tenantId;
            }
            return null;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value ??
                         User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        private async Task<MakePaymentViewModel> PrepareViewModelAsync(MakePaymentViewModel? existingVm = null)
        {
            var tenantId = GetTenantId();
            var invoices = tenantId.HasValue
                ? await _paymentService.GetAvailableInvoicesAsync(tenantId.Value)
                : new List<InvoiceDto>();

            return new MakePaymentViewModel
            {
                InvoiceId = existingVm?.InvoiceId ?? 0,
                Amount = existingVm?.Amount ?? 0,
                PaymentMethod = existingVm?.PaymentMethod ?? string.Empty,
                Notes = existingVm?.Notes ?? string.Empty,
                AvailableInvoices = invoices
            };
        }

        [HttpGet]
        public async Task<IActionResult> MakePayment()
        {
            var tenantId = GetTenantId();
            if (!tenantId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["PortalMode"] = "Tenant";
            var vm = await PrepareViewModelAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakePayment(MakePaymentViewModel vm)
        {
            ViewData["PortalMode"] = "Tenant";
            var tenantId = GetTenantId();
            if (!tenantId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                vm = await PrepareViewModelAsync(vm);
                return View(vm);
            }

            try
            {
                var dto = new MakePaymentRequestDto
                {
                    InvoiceId = vm.InvoiceId,
                    Amount = vm.Amount,
                    PaymentMethod = vm.PaymentMethod,
                    Notes = vm.Notes
                };

                var result = await _paymentService.MakePaymentAsync(tenantId.Value, dto);

                TempData["SuccessMessage"] = $"Payment successful! Payment ID: #{result.PaymentId}";
                return RedirectToAction("History");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                vm = await PrepareViewModelAsync(vm);
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            // Since system only has Admin, Member, Technician roles (no Landlord role),
            // detect portal mode based on context/referer or route
            var referer = Request.Headers["Referer"].ToString();
            var isFromLandlordContext = !string.IsNullOrEmpty(referer) && (
                referer.Contains("LandlordIndex") || 
                referer.Contains("MyProperties") ||
                referer.Contains("ManageViewings") ||
                referer.Contains("Invoice/Index") ||
                referer.Contains("Dashboard/LandlordIndex") ||
                referer.Contains("Property/MyProperties") ||
                referer.Contains("PropertyViewing/ManageViewings")
            );
            
            // Also check if current route is typically accessed from landlord portal
            // Payment/History is in landlordPages, so if accessed directly, assume landlord context
            // But we need to distinguish: if user has tenantId, they might be accessing as tenant
            var tenantId = GetTenantId();
            var hasTenantId = tenantId.HasValue;
            
            // If coming from landlord context OR (no referer and has tenantId means likely tenant access)
            // Actually, better: if has tenantId and NOT from landlord context, treat as tenant
            if (isFromLandlordContext)
            {
                ViewData["PortalMode"] = "Landlord";
                // For landlord, return empty list for now
                // TODO: Implement landlord payment view to show payments from their properties
                return View(new List<PaymentDto>());
            }
            else if (hasTenantId)
            {
                // Has tenantId and not from landlord context = tenant access
                ViewData["PortalMode"] = "Tenant";
                var history = await _paymentService.GetPaymentHistoryAsync(tenantId.Value);
                return View(history);
            }
            else
            {
                // No referer and no tenantId - could be landlord accessing directly
                // Let _AuthLayout detect from route (Payment/History is in landlordPages)
                // But return empty for now
                ViewData["PortalMode"] = "Landlord";
                return View(new List<PaymentDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ViewData["PortalMode"] = "Tenant";
            var tenantId = GetTenantId();
            if (!tenantId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                TempData["ErrorMessage"] = "Payment not found.";
                return RedirectToAction("History");
            }

            return View(payment);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadReceipt(int id)
        {
            var tenantId = GetTenantId();
            if (!tenantId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                TempData["ErrorMessage"] = "Payment not found.";
                return RedirectToAction("History");
            }

            // Generate a simple receipt as text
            var receiptContent = $@"
                =====================================
                        PAYMENT RECEIPT
                =====================================

                Payment ID: #{payment.PaymentId}
                Date: {payment.PaymentDate:dd/MM/yyyy HH:mm}

                Invoice: {payment.InvoiceNumber}
                Amount Paid: {payment.Amount:N0} VND
                Payment Method: {payment.PaymentMethod}
                Status: {payment.Status}

                -------------------------------------
                Invoice Summary
                -------------------------------------
                Total Amount: {payment.InvoiceTotalAmount:N0} VND
                Total Paid: {payment.InvoicePaidAmount:N0} VND
                Remaining: {payment.InvoiceRemainingAmount:N0} VND

                =====================================
                Thank you for your payment!
                =====================================
                ";

            var bytes = System.Text.Encoding.UTF8.GetBytes(receiptContent);
            return File(bytes, "text/plain", $"receipt-{payment.PaymentId}.txt");
        }

        [HttpGet]
        public async Task<IActionResult> LandlordIndex()
        {
            ViewData["PortalMode"] = "Landlord";
            var landlordId = GetCurrentUserId();
            if (landlordId <= 0)
            {
                TempData["ErrorMessage"] = "Please log in";
                return RedirectToAction("Login", "Auth");
            }

            var payments = await _paymentService.GetPaymentsByLandlordIdAsync(landlordId);
            
            var viewModel = new LandlordPaymentManagementViewModel
            {
                Payments = payments
            };

            // Calculate summary statistics
            var now = DateTime.Now;
            var currentMonth = now.Month;
            var currentYear = now.Year;
            var lastMonth = now.AddMonths(-1);

            // Total Collected (all confirmed payments)
            viewModel.TotalCollected = payments
                .Where(p => p.Status == "Confirmed")
                .Sum(p => p.Amount);

            // Pending Payments (pending status)
            viewModel.PendingPayments = payments
                .Where(p => p.Status == "Pending")
                .Sum(p => p.Amount);

            // Overdue (invoices with due date passed and not paid)
            viewModel.Overdue = payments
                .Where(p => p.DueDate < now && p.Status != "Confirmed")
                .Sum(p => p.Amount);

            // This Month (confirmed payments in current month)
            viewModel.ThisMonth = payments
                .Where(p => p.Status == "Confirmed" && 
                           p.PaidDate.HasValue &&
                           p.PaidDate.Value.Year == currentYear &&
                           p.PaidDate.Value.Month == currentMonth)
                .Sum(p => p.Amount);

            // Calculate change percentages (simplified - compare with last month)
            var lastMonthTotal = payments
                .Where(p => p.Status == "Confirmed" && 
                           p.PaidDate.HasValue &&
                           p.PaidDate.Value.Year == lastMonth.Year &&
                           p.PaidDate.Value.Month == lastMonth.Month)
                .Sum(p => p.Amount);

            var lastMonthThisMonth = payments
                .Where(p => p.Status == "Confirmed" && 
                           p.PaidDate.HasValue &&
                           p.PaidDate.Value.Year == lastMonth.Year &&
                           p.PaidDate.Value.Month == lastMonth.Month)
                .Sum(p => p.Amount);

            if (lastMonthTotal > 0)
            {
                viewModel.TotalCollectedChangePercent = ((viewModel.TotalCollected - lastMonthTotal) / lastMonthTotal) * 100;
            }

            if (lastMonthThisMonth > 0)
            {
                viewModel.ThisMonthChangePercent = ((viewModel.ThisMonth - lastMonthThisMonth) / lastMonthThisMonth) * 100;
            }

            // Calculate monthly revenue for last 6 months
            var months = new[] { "Aug", "Sep", "Oct", "Nov", "Dec", "Jan" };
            var monthNames = new[] { "August", "September", "October", "November", "December", "January" };
            var currentMonthIndex = now.Month - 1; // 0-based

            for (int i = 0; i < 6; i++)
            {
                var targetDate = now.AddMonths(-(5 - i));
                var monthRevenue = payments
                    .Where(p => p.Status == "Confirmed" &&
                               p.PaidDate.HasValue &&
                               p.PaidDate.Value.Year == targetDate.Year &&
                               p.PaidDate.Value.Month == targetDate.Month)
                    .Sum(p => p.Amount);

                viewModel.MonthlyRevenue.Add(new MonthlyRevenueData
                {
                    Month = months[i],
                    Amount = monthRevenue
                });
            }

            return View(viewModel);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
        {
            ViewData["PortalMode"] = "Landlord";
            var landlordId = GetCurrentUserId();
            if (landlordId <= 0)
            {
                return Json(new { success = false, message = "Please log in" });
            }

            try
            {
                var result = await _paymentService.ConfirmPaymentAsync(request.PaymentId, landlordId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Payment confirmed successfully.";
                    return Json(new { success = true, message = "Payment confirmed successfully." });
                }
                return Json(new { success = false, message = "Failed to confirm payment." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class ConfirmPaymentRequest
        {
            public int PaymentId { get; set; }
        }

        [HttpGet]
        public IActionResult GenerateBankTransferQr(int invoiceId, decimal amount)
        {
            string myBank = "Vietcombank";
            string myAccount = "1025755773";
            string myName = "TRUONG HOANG PHAT";
            string content = $"THANH TOAN HD {invoiceId}";

            string qrText = $"NGAN HANG: {myBank}\n" +
                            $"STK: {myAccount}\n" +
                            $"CHU TK: {myName}\n" +
                            $"SO TIEN: {amount:N0} VND\n" +
                            $"NOI DUNG: {content}";

            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrData);

            using var qrImage = qrCode.GetGraphic(10);

            using var ms = new MemoryStream();
            qrImage.Save(ms, ImageFormat.Png);
            return File(ms.ToArray(), "image/png");
        }
    }
}
