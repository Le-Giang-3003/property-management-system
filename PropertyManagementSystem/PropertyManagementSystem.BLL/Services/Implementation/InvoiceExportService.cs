using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.BLL.DTOs.Invoice;

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
            throw new KeyNotFoundException($"Invoice {invoiceId} không tồn tại");

        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Text($"INVOICE #{invoice.InvoiceNumber}")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    // Thông tin chung
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Type: {invoice.InvoiceType}");
                        row.RelativeItem().AlignRight()
                           .Text($"Status: {invoice.Status}");
                    });

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Issue date: {invoice.IssueDate:dd/MM/yyyy}");
                        row.RelativeItem().AlignRight()
                           .Text($"Due date: {invoice.DueDate:dd/MM/yyyy}");
                    });

                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    // Bảng tiền
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(120);
                        });

                        void Header(string label)
                        {
                            table.Cell().Element(e => e.PaddingVertical(5))
                                .Text(label).SemiBold();
                        }

                        void Row(string label, decimal value)
                        {
                            table.Cell().Element(e => e.PaddingVertical(3))
                                .Text(label);

                            table.Cell().AlignRight().Element(e => e.PaddingVertical(3))
                                .Text($"{value:N0}"); // số nguyên, có thể đổi format
                        }

                        Header("Item");
                        Header("Amount");

                        Row("Amount", invoice.Amount);
                        Row("Tax", invoice.TaxAmount);
                        Row("Discount", -invoice.DiscountAmount);
                        Row("Total", invoice.TotalAmount);
                        Row("Paid", invoice.PaidAmount);
                        Row("Remaining", invoice.RemainingAmount);
                    });

                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    // Mô tả
                    if (!string.IsNullOrWhiteSpace(invoice.Description))
                    {
                        col.Item().Text(text =>
                        {
                            text.Span("Description: ").SemiBold();
                            text.Span(invoice.Description);
                        });
                    }
                });

                page.Footer()
                    .AlignCenter()
                    .Text($"Generated at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Medium);
            });
        });

        return document.GeneratePdf();
    }
}
