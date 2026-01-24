using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.Services.Interface;

namespace PropertyManagementSystem.Web.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IInvoiceExportService _invoiceExportService;

        public InvoiceController(
            IInvoiceService invoiceService,
            IInvoiceExportService invoiceExportService)
        {
            _invoiceService = invoiceService;
            _invoiceExportService = invoiceExportService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int tenantId = 1; // tạm cứng, sau lấy từ user login
            var invoices = await _invoiceService.GetAvailableInvoicesByTenantAsync(tenantId);
            return View(invoices);   // => View: Views/Invoice/Index.cshtml
        }

        [HttpGet]
        public async Task<IActionResult> ExportPdf(int id)
        {
            try
            {
                var bytes = await _invoiceExportService.ExportToPdfAsync(id);
                var fileName = $"Invoice-{id}.pdf";
                return File(bytes, "application/pdf", fileName);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
