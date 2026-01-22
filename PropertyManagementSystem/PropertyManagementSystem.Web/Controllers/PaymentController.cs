using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.DTOs.Payments;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.Web.ViewModels.Payment;
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
            int tenantId = 1;

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

            int tenantId = 1;

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
                return RedirectToAction("History");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }
        [HttpGet]
        public async Task<IActionResult> History()
        {
            // TODO: Load payment history của tenant hiện tại
            return View();
        }
    }
}
