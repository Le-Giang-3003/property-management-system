using System.Text;
using PropertyManagementSystem.BLL.Services.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class InvoiceExportService : IInvoiceExportService
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceExportService(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        public async Task<byte[]> ExportToPdfAsync(int invoiceId)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
                throw new Exception("Invoice không tồn tại");

            // Tạm thời: xuất “PDF” dạng text để test luồng
                            var content = $@"
                INVOICE {invoice.InvoiceNumber}
                LeaseId: {invoice.LeaseId}
                IssueDate: {invoice.IssueDate:dd/MM/yyyy}
                DueDate: {invoice.DueDate:dd/MM/yyyy}
                Total: {invoice.TotalAmount}
                Paid: {invoice.PaidAmount}
                Remaining: {invoice.RemainingAmount}
                Status: {invoice.Status}
                ";
            return Encoding.UTF8.GetBytes(content);
        }
    }
}
