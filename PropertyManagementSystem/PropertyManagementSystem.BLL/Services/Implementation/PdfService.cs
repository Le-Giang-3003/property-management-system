using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class PdfService : IPdfService
    {
        private readonly IUnitOfWork _unitOfWork;

        // ✅ CONSTRUCTOR
        public PdfService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<byte[]> GenerateLeasePdfAsync(Lease lease)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // ✅ LẤY CHỮ KÝ
            var signatures = await _unitOfWork.LeaseSignatures.GetByLeaseIdAsync(lease.LeaseId);
            var landlordSig = signatures.FirstOrDefault(s => s.SignerRole == "Landlord");
            var tenantSig = signatures.FirstOrDefault(s => s.SignerRole == "Tenant");

            var pdfBytes = global::QuestPDF.Fluent.Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // HEADER
                    page.Header().Column(col =>
                    {
                        col.Item().Text("HỢP ĐỒNG THUÊ NHÀ")
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.Blue.Medium)
                            .AlignCenter();

                        col.Item().Text($"Ngày tạo: {DateTime.Now:dd/MM/yyyy}")
                            .FontSize(9)
                            .AlignRight();
                    });

                    // CONTENT
                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        column.Spacing(3);

                        column.Item().Text($"Số hợp đồng: {lease.LeaseNumber}").Bold().FontSize(10);
                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // I. BÊN CHO THUÊ
                        column.Item().PaddingTop(8).Text("I. BÊN CHO THUÊ (BÊN A)")
                            .FontSize(10)
                            .Bold()
                            .FontColor(Colors.Blue.Medium);

                        column.Item().Text($"{lease.Property?.Landlord?.FullName ?? "N/A"} | {lease.Property?.Landlord?.PhoneNumber ?? "N/A"}").FontSize(9);

                        // II. BÊN THUÊ
                        column.Item().PaddingTop(6).Text("II. BÊN THUÊ (BÊN B)")
                            .FontSize(10)
                            .Bold()
                            .FontColor(Colors.Blue.Medium);

                        column.Item().Text($"{lease.Tenant?.FullName ?? "N/A"} | {lease.Tenant?.PhoneNumber ?? "N/A"}").FontSize(9);

                        // III. THÔNG TIN BẤT ĐỘNG SẢN
                        column.Item().PaddingTop(6).Text("III. THÔNG TIN BẤT ĐỘNG SẢN")
                            .FontSize(10)
                            .Bold()
                            .FontColor(Colors.Blue.Medium);

                        column.Item().Text($"{lease.Property?.Name} - {lease.Property?.Address}").FontSize(9);
                        column.Item().Text($"Diện tích: {lease.Property?.SquareFeet} m² | Phòng: {lease.Property?.Bedrooms}/{lease.Property?.Bathrooms}").FontSize(9);

                        // IV. THÔNG TIN TÀI CHÍNH
                        column.Item().PaddingTop(6).Text("IV. THÔNG TIN TÀI CHÍNH")
                            .FontSize(10)
                            .Bold()
                            .FontColor(Colors.Blue.Medium);

                        column.Item().Text($"Tiền thuê: {lease.MonthlyRent:N0} VNĐ/tháng | Đặt cọc: {lease.SecurityDeposit:N0} VNĐ").FontSize(9);
                        column.Item().Text($"Thanh toán: Ngày {lease.PaymentDueDay} hàng tháng").FontSize(9);
                        column.Item().Text($"Thời hạn: {lease.StartDate:dd/MM/yyyy} - {lease.EndDate:dd/MM/yyyy}").FontSize(9);

                        // V. ĐIỀU KHOẢN HỢP ĐỒNG
                        column.Item().PaddingTop(8).Text("V. ĐIỀU KHOẢN HỢP ĐỒNG")
                            .FontSize(10)
                            .Bold()
                            .FontColor(Colors.Blue.Medium);

                        column.Item()
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(8)
                            .Text(lease.Terms ?? "Không có điều khoản")
                            .FontSize(8)
                            .LineHeight(1.3f);

                        // VI. ĐIỀU KIỆN ĐẶC BIỆT
                        if (!string.IsNullOrEmpty(lease.SpecialConditions))
                        {
                            column.Item().PaddingTop(6).Text("VI. ĐIỀU KIỆN ĐẶC BIỆT")
                                .FontSize(10)
                                .Bold()
                                .FontColor(Colors.Orange.Medium);

                            column.Item()
                                .Border(1)
                                .BorderColor(Colors.Orange.Lighten2)
                                .Padding(8)
                                .Text(lease.SpecialConditions)
                                .FontSize(8)
                                .LineHeight(1.3f);
                        }

                        // ✅ VII. CHỮ KÝ - CẢI TIẾN
                        
                            column.Item().PaddingTop(15).Row(row =>
                            {
                                // BÊN CHO THUÊ
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().AlignCenter().Text("BÊN CHO THUÊ").Bold().FontSize(9);

                                    if (landlordSig != null)
                                    {
                                        col.Item().AlignCenter().Text(landlordSig.User?.FullName ?? "").FontSize(8);

                                        // Hiển thị ảnh chữ ký nếu có
                                        if (!string.IsNullOrEmpty(landlordSig.SignatureData))
                                        {
                                            try
                                            {
                                                var base64Data = landlordSig.SignatureData.Contains(",")
                                                    ? landlordSig.SignatureData.Split(',')[1]
                                                    : landlordSig.SignatureData;
                                                var imageBytes = Convert.FromBase64String(base64Data);
                                                col.Item().AlignCenter().Width(120).Image(imageBytes);
                                            }
                                            catch
                                            {
                                                col.Item().PaddingTop(20).AlignCenter()
                                                    .Text("_____________________")
                                                    .FontSize(8);
                                            }
                                        }
                                        else
                                        {
                                            col.Item().PaddingTop(20).AlignCenter()
                                                .Text("_____________________")
                                                .FontSize(8);
                                        }

                                        col.Item().AlignCenter().Text($"Ký ngày: {landlordSig.SignedAt:dd/MM/yyyy}").FontSize(7);
                                    }
                                    else
                                    {
                                        col.Item().PaddingTop(30).AlignCenter()
                                            .Text("_____________________")
                                            .FontSize(8);
                                    }
                                });

                                // BÊN THUÊ
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().AlignCenter().Text("BÊN THUÊ").Bold().FontSize(9);

                                    if (tenantSig != null)
                                    {
                                        col.Item().AlignCenter().Text(tenantSig.User?.FullName ?? "").FontSize(8);

                                        // Hiển thị ảnh chữ ký nếu có
                                        if (!string.IsNullOrEmpty(tenantSig.SignatureData))
                                        {
                                            try
                                            {
                                                var base64Data = tenantSig.SignatureData.Contains(",")
                                                    ? tenantSig.SignatureData.Split(',')[1]
                                                    : tenantSig.SignatureData;
                                                var imageBytes = Convert.FromBase64String(base64Data);
                                                col.Item().AlignCenter().Width(120).Image(imageBytes);
                                            }
                                            catch
                                            {
                                                col.Item().PaddingTop(20).AlignCenter()
                                                    .Text("_____________________")
                                                    .FontSize(8);
                                            }
                                        }
                                        else
                                        {
                                            col.Item().PaddingTop(20).AlignCenter()
                                                .Text("_____________________")
                                                .FontSize(8);
                                        }

                                        col.Item().AlignCenter().Text($"Ký ngày: {tenantSig.SignedAt:dd/MM/yyyy}").FontSize(7);
                                    }
                                    else
                                    {
                                        col.Item().PaddingTop(30).AlignCenter()
                                            .Text("_____________________")
                                            .FontSize(8);
                                        
                                    }
                                });
                            });

                            // Hiển thị trạng thái hợp đồng
                            if (lease.Status == "Active" && lease.SignedDate.HasValue)
                            {
                                column.Item().PaddingTop(5).AlignCenter()
                                    .Text($"Hợp đồng có hiệu lực từ ngày {lease.SignedDate.Value:dd/MM/yyyy}")
                                    .FontSize(8)
                                    .Italic()
                                    .FontColor(Colors.Green.Medium);
                            }
                    });

                    // FOOTER
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Trang ");
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            }).GeneratePdf();

            return pdfBytes;
        }
    }
}
