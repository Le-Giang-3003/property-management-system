using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Payments;
using PropertyManagementSystem.BLL.DTOs.Invoice;
using PropertyManagementSystem.Web.ViewModels.Payment;
using QRCoder;
using System.Drawing.Imaging;

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
            ViewData["PortalMode"] = "Tenant";
            var tenantId = GetTenantId();
            if (!tenantId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            var history = await _paymentService.GetPaymentHistoryAsync(tenantId.Value);
            return View(history);
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
