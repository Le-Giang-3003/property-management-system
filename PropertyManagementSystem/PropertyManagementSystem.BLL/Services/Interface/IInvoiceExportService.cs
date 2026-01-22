namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IInvoiceExportService
    {
        Task<byte[]> ExportToPdfAsync(int invoiceId);
    }
}
