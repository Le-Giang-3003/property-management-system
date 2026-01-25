using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Payments;
using PropertyManagementSystem.Web.ViewModels.Payment;
using QRCoder;
using System.Drawing.Imaging;
using System.Security.Claims;

namespace PropertyManagementSystem.Web.Controllers
{
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

        [HttpGet]
        [Route("Payment/MakePayment")]
        public async Task<IActionResult> MakePayment()
        {
            var tenantId = GetTenantId();
            if (tenantId == null) return RedirectToAction("Login", "Auth");

            var vm = new MakePaymentViewModel
            {
                AvailableInvoices = await _paymentService.GetAvailableInvoicesAsync(tenantId.Value)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Payment/MakePayment")]
        public async Task<IActionResult> MakePayment(MakePaymentViewModel vm)
        {
            var tenantId = GetTenantId();
            if (tenantId == null) return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                vm.AvailableInvoices = await _paymentService.GetAvailableInvoicesAsync(tenantId.Value);
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

                TempData["SuccessMessage"] = "Thanh toán thành công!";
                return RedirectToAction("History");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                vm.AvailableInvoices = await _paymentService.GetAvailableInvoicesAsync(tenantId.Value);
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var tenantId = GetTenantId();
            if (tenantId == null) return RedirectToAction("Login", "Auth");

            var history = await _paymentService.GetPaymentHistoryAsync(tenantId.Value);
            return View(history);
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

        [HttpGet]
        public async Task<IActionResult> Report(DateTime? fromDate, DateTime? toDate, string? paymentMethod)
        {
            bool isAdmin = User.IsInRole("Admin") ||
                           User.FindFirst("RoleId")?.Value == "1" ||
                           User.FindFirst(ClaimTypes.Role)?.Value == "Admin" ||
                           User.FindFirst("role")?.Value == "Admin";

            int roleId = isAdmin ? 1 : 0;

            var tenantIdClaim = User.FindFirst("TenantId")?.Value;
            int tenantId = string.IsNullOrEmpty(tenantIdClaim) ? 0 : int.Parse(tenantIdClaim);

            var start = fromDate ?? DateTime.Today.AddMonths(-1);
            var end = toDate ?? DateTime.Today;

            var reportData = await _paymentService.GetPaymentReportAsync(tenantId, roleId, start, end, paymentMethod);

            var vm = new PaymentReportViewModel
            {
                FromDate = reportData.FromDate,
                ToDate = reportData.ToDate,
                SelectedPaymentMethod = reportData.SelectedMethod,
                TotalAmountPaid = reportData.TotalPaid,
                TransactionCount = reportData.TotalTransactions,
                PaymentHistory = reportData.Payments,
                TenantName = reportData.TenantName
            };
            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("Payment/ManageDisputes")]
        public async Task<IActionResult> ManageDisputes()
        {
            var allDisputes = await _paymentService.GetAllDisputesAsync();

            var vm = new AdminDisputeManagementViewModel
            {
                PendingDisputes = allDisputes.Where(d => d.Status == "Pending").ToList(),
                ResolvedDisputes = allDisputes.Where(d => d.Status != "Pending").ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RaiseDispute(RaiseDisputeViewModel vm)
        {
            if (!ModelState.IsValid) return RedirectToAction("History");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var dto = new RaiseDisputeDTO
            { // Dùng DTO viết hoa cho khớp Service của bạn
                InvoiceId = vm.InvoiceId,
                Reason = vm.Reason,
                Description = vm.Description
            };

            if (await _paymentService.RaiseDisputeAsync(userId, dto))
                TempData["SuccessMessage"] = "Khiếu nại đã được gửi.";

            return RedirectToAction("History");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResolveDispute(ResolveDisputeViewModel vm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (vm.IsRefundRequest)
            {
                await _paymentService.ProcessRefundAsync(vm.DisputeId, adminId, vm.Resolution);
            }
            else
            {
                var dto = new ResolveDisputeDTO
                {
                    DisputeId = vm.DisputeId,
                    Resolution = vm.Resolution,
                    Status = vm.Status
                };
                await _paymentService.ResolveDisputeAsync(adminId, dto);
            }

             TempData["SuccessMessage"] = "Xử lý khiếu nại thành công!";
            return RedirectToAction("ManageDisputes");
        }

    }
}
