using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManagementSystem.BLL.DTOs;
using PropertyManagementSystem.BLL.Services.Interface;

namespace PropertyManagementSystem.Web.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(IDocumentService documentService, ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        #region Upload

        [HttpGet]
        public IActionResult Upload(string? entityType, int? entityId)
        {
            ViewBag.EntityType = entityType ?? "";
            ViewBag.EntityId = entityId ?? 0;
            ViewBag.DocumentTypes = GetDocumentTypesSelectList();
            ViewBag.EntityTypes = GetEntityTypesSelectList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(
            IFormFile file,
            string documentType,
            string entityType,
            int entityId,
            string? description,
            string? returnUrl)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    TempData["Error"] = "❌ Vui lòng chọn file";
                    return RedirectToAction(nameof(Upload), new { entityType, entityId });
                }

                var userId = GetCurrentUserId();
                var isAdmin = User.IsInRole("Admin");

                // Validate file type
                if (!_documentService.IsValidFileType(file.FileName, documentType, isAdmin))
                {
                    TempData["Error"] = $"❌ Loại file không được phép cho {documentType}";
                    return RedirectToAction(nameof(Upload), new { entityType, entityId });
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileType = _documentService.GetFileType(extension);

                // Validate file size
                if (!_documentService.ValidateFileSize(file.Length, fileType))
                {
                    var maxSize = GetMaxSizeForFileType(fileType);
                    TempData["Error"] = $"❌ File vượt quá dung lượng cho phép ({maxSize})";
                    return RedirectToAction(nameof(Upload), new { entityType, entityId });
                }

                // Convert IFormFile to DTO
                var fileDto = new FileUploadDto
                {
                    FileStream = file.OpenReadStream(),
                    FileName = file.FileName,
                    FileSize = file.Length,
                    ContentType = file.ContentType
                };

                // Upload document
                var document = await _documentService.UploadDocumentAsync(
                    fileDto, documentType, entityType, entityId, userId, description);

                if (document != null)
                {
                    TempData["Success"] = $"✅ Upload '{file.FileName}' thành công! ({FormatFileSize(file.Length)})";

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction(nameof(ListByEntity), new { entityType, entityId });
                }

                TempData["Error"] = "❌ Không thể upload file";
                return RedirectToAction(nameof(Upload), new { entityType, entityId });
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = "❌ " + ex.Message;
                return RedirectToAction(nameof(Upload), new { entityType, entityId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document");
                TempData["Error"] = "❌ Có lỗi xảy ra khi upload file";
                return RedirectToAction(nameof(Upload), new { entityType, entityId });
            }
        }

        #endregion

        #region Download

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var result = await _documentService.DownloadDocumentAsync(id);

                if (result == null)
                {
                    TempData["Error"] = "❌ Không tìm thấy file hoặc file đã bị xóa";
                    return RedirectToAction(nameof(MyDocuments));
                }

                var (fileBytes, contentType, fileName) = result.Value;

                _logger.LogInformation("User {UserId} downloaded document {DocumentId}", GetCurrentUserId(), id);

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading document {DocumentId}", id);
                TempData["Error"] = "❌ Có lỗi xảy ra khi tải file";
                return RedirectToAction(nameof(MyDocuments));
            }
        }

        #endregion

        #region Delete

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? returnUrl)
        {
            try
            {
                var userId = GetCurrentUserId();
                var isAdmin = User.IsInRole("Admin");

                var document = await _documentService.GetDocumentByIdAsync(id);
                if (document == null)
                {
                    TempData["Error"] = "❌ Không tìm thấy tài liệu";
                    return RedirectToUrl(returnUrl, nameof(MyDocuments));
                }

                var result = await _documentService.DeleteDocumentAsync(id, userId, isAdmin);

                if (result)
                {
                    TempData["Success"] = $"✅ Đã xóa '{document.FileName}' thành công";
                }
                else
                {
                    TempData["Error"] = "❌ Không thể xóa tài liệu";
                }

                return RedirectToUrl(returnUrl, nameof(MyDocuments));
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["Error"] = "❌ " + ex.Message;
                return RedirectToUrl(returnUrl, nameof(MyDocuments));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document {DocumentId}", id);
                TempData["Error"] = "❌ Có lỗi xảy ra khi xóa tài liệu";
                return RedirectToUrl(returnUrl, nameof(MyDocuments));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var document = await _documentService.GetDocumentByIdAsync(id);

                if (document == null)
                {
                    TempData["Error"] = "❌ Không tìm thấy tài liệu";
                    return RedirectToAction(nameof(DeletedDocuments));
                }

                var result = await _documentService.PermanentDeleteAsync(id, userId, true);

                if (result)
                {
                    TempData["Success"] = $"✅ Đã xóa vĩnh viễn '{document.FileName}'";
                }
                else
                {
                    TempData["Error"] = "❌ Không thể xóa tài liệu";
                }

                return RedirectToAction(nameof(DeletedDocuments));
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["Error"] = "❌ " + ex.Message;
                return RedirectToAction(nameof(DeletedDocuments));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error permanently deleting document {DocumentId}", id);
                TempData["Error"] = "❌ Có lỗi xảy ra";
                return RedirectToAction(nameof(DeletedDocuments));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var document = await _documentService.GetDocumentByIdAsync(id);
                if (document == null)
                {
                    TempData["Error"] = "❌ Không tìm thấy tài liệu";
                    return RedirectToAction(nameof(DeletedDocuments));
                }

                var result = await _documentService.RestoreDocumentAsync(id);

                if (result)
                {
                    TempData["Success"] = $"✅ Đã khôi phục '{document.FileName}'";
                }
                else
                {
                    TempData["Error"] = "❌ Không thể khôi phục";
                }

                return RedirectToAction(nameof(DeletedDocuments));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring document {DocumentId}", id);
                TempData["Error"] = "❌ Có lỗi xảy ra";
                return RedirectToAction(nameof(DeletedDocuments));
            }
        }

        #endregion

        #region List Views

        [HttpGet]
        public async Task<IActionResult> MyDocuments()
        {
            try
            {
                var userId = GetCurrentUserId();
                var documents = await _documentService.GetDocumentsByUserAsync(userId);

                return View(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading documents for user {UserId}", GetCurrentUserId());
                TempData["Error"] = "❌ Có lỗi xảy ra khi tải danh sách tài liệu";
                return View(new List<DAL.Entities.Document>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListByEntity(string entityType, int entityId)
        {
            try
            {
                var documents = await _documentService.GetDocumentsByEntityAsync(entityType, entityId);

                ViewBag.EntityType = entityType;
                ViewBag.EntityId = entityId;
                ViewBag.EntityDisplayName = GetEntityDisplayName(entityType);

                return View(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading documents for {EntityType} {EntityId}", entityType, entityId);
                TempData["Error"] = "❌ Có lỗi xảy ra khi tải danh sách tài liệu";
                return View(new List<DAL.Entities.Document>());
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> DeletedDocuments()
        {
            try
            {
                var documents = await _documentService.GetDeletedDocumentsAsync();
                return View(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading deleted documents");
                TempData["Error"] = "❌ Có lỗi xảy ra";
                return View(new List<DAL.Entities.Document>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var document = await _documentService.GetDocumentByIdAsync(id);
                if (document == null)
                {
                    TempData["Error"] = "❌ Không tìm thấy tài liệu";
                    return RedirectToAction(nameof(MyDocuments));
                }

                return View(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading document {DocumentId}", id);
                TempData["Error"] = "❌ Có lỗi xảy ra";
                return RedirectToAction(nameof(MyDocuments));
            }
        }

        #endregion

        #region Helper Methods

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value ??
                         User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        private List<SelectListItem> GetDocumentTypesSelectList()
        {
            return new List<SelectListItem>
            {
                new() { Value = "Contract", Text = "Hợp đồng (Contract)" },
                new() { Value = "Invoice", Text = "Hóa đơn (Invoice)" },
                new() { Value = "Receipt", Text = "Biên lai (Receipt)" },
                new() { Value = "ID", Text = "Giấy tờ tùy thân (ID)" },
                new() { Value = "Application", Text = "Đơn đăng ký (Application)" },
                new() { Value = "Image", Text = "Hình ảnh (Image)" },
                new() { Value = "Video", Text = "Video" },
                new() { Value = "Spreadsheet", Text = "Bảng tính (Spreadsheet)" },
                new() { Value = "Presentation", Text = "Trình chiếu (Presentation)" },
                new() { Value = "Other", Text = "Khác (Other)" }
            };
        }

        private List<SelectListItem> GetEntityTypesSelectList()
        {
            return new List<SelectListItem>
            {
                new() { Value = "Property", Text = "Bất động sản (Property)" },
                new() { Value = "Lease", Text = "Hợp đồng thuê (Lease)" },
                new() { Value = "Invoice", Text = "Hóa đơn (Invoice)" },
                new() { Value = "Payment", Text = "Thanh toán (Payment)" },
                new() { Value = "MaintenanceRequest", Text = "Yêu cầu bảo trì (Maintenance)" },
                new() { Value = "RentalApplication", Text = "Đơn thuê nhà (Application)" }
            };
        }

        private string GetEntityDisplayName(string entityType)
        {
            return entityType switch
            {
                "Property" => "Bất động sản",
                "Lease" => "Hợp đồng thuê",
                "Invoice" => "Hóa đơn",
                "Payment" => "Thanh toán",
                "MaintenanceRequest" => "Yêu cầu bảo trì",
                "RentalApplication" => "Đơn thuê nhà",
                _ => entityType
            };
        }

        private string GetMaxSizeForFileType(string fileType)
        {
            return fileType.ToLowerInvariant() switch
            {
                "jpg" or "jpeg" or "png" or "webp" => "5MB",
                "mp4" => "50MB",
                "pdf" or "docx" or "xlsx" or "pptx" or "csv" => "10MB",
                "zip" => "20MB",
                _ => "10MB"
            };
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private IActionResult RedirectToUrl(string? returnUrl, string defaultAction)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(defaultAction);
        }

        #endregion
    }
}
