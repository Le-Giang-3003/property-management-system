using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Payments;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.Web.ViewModels.Payment;
using QRCoder;
using System.Drawing.Imaging;
using System.Security.Claims;

namespace PropertyManagementSystem.Web.Controllers
{
    //[Authorize(Roles = "Tenant")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> MakePayment()
        {
            var tenantClaim = User.FindFirst("TenantId");
            if (tenantClaim == null || !int.TryParse(tenantClaim.Value, out var tenantId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var invoices = await _paymentService.GetAvailableInvoicesAsync(tenantId);

            var vm = new MakePaymentViewModel
            {
                AvailableInvoices = invoices
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakePayment(MakePaymentViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var tenantClaim = User.FindFirst("TenantId");
            if (tenantClaim == null || !int.TryParse(tenantClaim.Value, out var tenantId))
            {
                return RedirectToAction("Login", "Auth");
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

                var result = await _paymentService.MakePaymentAsync(tenantId, dto);

                TempData["SuccessMessage"] = $"Thanh toán thành công. Mã thanh toán: {result.PaymentId}";
                return RedirectToAction("History", new { invoiceId = vm.InvoiceId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(vm);
            }
        }
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var tenantClaim = User.FindFirst("TenantId");
            if (tenantClaim == null || !int.TryParse(tenantClaim.Value, out var tenantId))
                return RedirectToAction("Login", "Auth");

            var history = await _paymentService.GetPaymentHistoryAsync(tenantId);
            return View(history);
        }

        public IActionResult GenerateBankTransferQr(int invoiceId, decimal amount)
        {
            // TODO: lấy thông tin invoice, tenant, nội dung chuyển khoản, số tài khoản thực tế
            var accountNumber = "1025755773 ";
            var bankName = "Vietcombank";
            var content = $"Thanh toan hoa don {invoiceId}";

            // Chuỗi đơn giản, sau này đổi sang format VietQR
            var qrContent = $"BANK:{bankName}|ACC:{accountNumber}|AMT:{amount}|MSG:{content}";

            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrData);
            using var qrImage = qrCode.GetGraphic(20);

            using var ms = new MemoryStream();
            qrImage.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            return File(ms.ToArray(), "image/png");
        }
    }
}
