using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Payments;
using PropertyManagementSystem.Web.ViewModels.Payment;
using QRCoder;
using System.Drawing.Imaging;

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
    }
}
