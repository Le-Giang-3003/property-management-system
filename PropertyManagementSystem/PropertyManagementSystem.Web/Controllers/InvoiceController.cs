using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.Services.Interface;

public class InvoiceController : Controller
{
    private readonly IInvoiceExportService _invoiceExportService;

    public InvoiceController(IInvoiceExportService invoiceExportService)
    {
        _invoiceExportService = invoiceExportService;
    }

    [HttpGet]
    public async Task<IActionResult> ExportPdf(int id)
    {
        try
        {
            var bytes = await _invoiceExportService.ExportToPdfAsync(id);
            return File(bytes, "text/plain", $"Invoice-{id}.txt");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

}
