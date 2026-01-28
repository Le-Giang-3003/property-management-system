using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.Web.Controllers
{
    /// <summary>
    /// Controller for testing automatic invoice generation.
    /// In production, this should be removed or restricted to Admin only.
    /// </summary>
    [Authorize] // Allow any authenticated user for testing
    public class InvoiceGenerationController : Controller
    {
        private readonly ILeaseRepository _leaseRepository;
        private readonly IInvoiceService _invoiceService;
        private readonly IEmailService _emailService;
        private readonly ILogger<InvoiceGenerationController> _logger;

        public InvoiceGenerationController(
            ILeaseRepository leaseRepository,
            IInvoiceService invoiceService,
            IEmailService emailService,
            ILogger<InvoiceGenerationController> logger)
        {
            _leaseRepository = leaseRepository;
            _invoiceService = invoiceService;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Test page for invoice generation
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var activeLeases = await _leaseRepository.GetAllActiveLeasesAsync();
                return View(activeLeases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading InvoiceGeneration Index");
                return Content($"Error: {ex.Message}\n\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Simple test endpoint to verify controller is working
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Allow without login for testing
        public IActionResult Test()
        {
            return Json(new {
                success = true,
                message = "InvoiceGeneration controller is working!",
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        /// <summary>
        /// Trigger invoice generation for a specific date (for testing)
        /// Example: /InvoiceGeneration/TriggerForDate?simulatedDate=2026-01-31
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> TriggerForDate(DateTime simulatedDate, bool sendEmail = false)
        {
            var results = new List<object>();

            try
            {
                _logger.LogInformation("Manual invoice generation triggered for date: {Date}", simulatedDate);

                // First, update any overdue invoices
                await _invoiceService.UpdateOverdueInvoicesAsync();

                // Get all active leases
                var activeLeases = await _leaseRepository.GetAllActiveLeasesAsync();
                var billingMonth = new DateTime(simulatedDate.Year, simulatedDate.Month, 1);

                int invoicesCreated = 0;
                int invoicesSkipped = 0;

                foreach (var lease in activeLeases)
                {
                    try
                    {
                        // Check if simulated date matches the StartDate day of the lease
                        if (simulatedDate.Day != lease.StartDate.Day)
                        {
                            results.Add(new
                            {
                                LeaseId = lease.LeaseId,
                                LeaseNumber = lease.LeaseNumber,
                                TenantName = lease.Tenant?.FullName,
                                PropertyName = lease.Property?.Name,
                                StartDateDay = lease.StartDate.Day,
                                SimulatedDay = simulatedDate.Day,
                                Status = "Skipped",
                                Reason = $"Day doesn't match (Lease StartDate.Day={lease.StartDate.Day}, SimulatedDate.Day={simulatedDate.Day})"
                            });
                            continue;
                        }

                        // Check if lease is within its period on the simulated date
                        if (simulatedDate.Date < lease.StartDate.Date || simulatedDate.Date > lease.EndDate.Date)
                        {
                            results.Add(new
                            {
                                LeaseId = lease.LeaseId,
                                LeaseNumber = lease.LeaseNumber,
                                TenantName = lease.Tenant?.FullName,
                                PropertyName = lease.Property?.Name,
                                StartDate = lease.StartDate,
                                EndDate = lease.EndDate,
                                SimulatedDate = simulatedDate,
                                Status = "Skipped",
                                Reason = "Simulated date is outside lease period"
                            });
                            continue;
                        }

                        // Create invoice for the billing month
                        var invoice = await _invoiceService.CreateMonthlyInvoiceAsync(lease, billingMonth);

                        if (invoice != null)
                        {
                            invoicesCreated++;

                            var result = new
                            {
                                LeaseId = lease.LeaseId,
                                LeaseNumber = lease.LeaseNumber,
                                TenantName = lease.Tenant?.FullName,
                                TenantEmail = lease.Tenant?.Email,
                                PropertyName = lease.Property?.Name,
                                LandlordEmail = lease.Property?.Landlord?.Email,
                                InvoiceId = invoice.InvoiceId,
                                InvoiceNumber = invoice.InvoiceNumber,
                                BillingMonth = invoice.BillingMonthDisplay,
                                Amount = invoice.TotalAmount,
                                DueDate = invoice.DueDate,
                                Status = "Created",
                                EmailSent = false,
                                EmailError = (string?)null
                            };

                            // Send email if requested
                            if (sendEmail)
                            {
                                var invoiceEntity = new DAL.Entities.Invoice
                                {
                                    InvoiceId = invoice.InvoiceId,
                                    InvoiceNumber = invoice.InvoiceNumber,
                                    IssueDate = invoice.IssueDate,
                                    DueDate = invoice.DueDate,
                                    TotalAmount = invoice.TotalAmount,
                                    BillingMonth = invoice.BillingMonth
                                };

                                try
                                {
                                    await _emailService.SendInvoiceCreatedToTenantAsync(invoiceEntity, lease);
                                    await _emailService.SendInvoiceCreatedToLandlordAsync(invoiceEntity, lease);
                                    result = result with { EmailSent = true };
                                }
                                catch (Exception emailEx)
                                {
                                    result = result with { EmailError = emailEx.Message };
                                }
                            }

                            results.Add(result);
                        }
                        else
                        {
                            invoicesSkipped++;
                            results.Add(new
                            {
                                LeaseId = lease.LeaseId,
                                LeaseNumber = lease.LeaseNumber,
                                TenantName = lease.Tenant?.FullName,
                                PropertyName = lease.Property?.Name,
                                BillingMonth = billingMonth.ToString("MM/yyyy"),
                                Status = "Skipped",
                                Reason = "Invoice already exists for this month"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add(new
                        {
                            LeaseId = lease.LeaseId,
                            LeaseNumber = lease.LeaseNumber,
                            Status = "Error",
                            Error = ex.Message
                        });
                    }
                }

                return Json(new
                {
                    success = true,
                    simulatedDate = simulatedDate.ToString("yyyy-MM-dd"),
                    billingMonth = billingMonth.ToString("MM/yyyy"),
                    summary = new
                    {
                        totalLeases = activeLeases.Count(),
                        invoicesCreated,
                        invoicesSkipped
                    },
                    details = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during manual invoice generation");
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    details = results
                });
            }
        }

        /// <summary>
        /// Create invoice for a specific lease (for testing)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateForLease(int leaseId, DateTime billingMonth, bool sendEmail = false)
        {
            try
            {
                var lease = await _leaseRepository.GetByIdAsync(leaseId);
                if (lease == null)
                {
                    return Json(new { success = false, error = "Lease not found" });
                }

                // Reload lease with all navigation properties
                var activeLeases = await _leaseRepository.GetAllActiveLeasesAsync();
                lease = activeLeases.FirstOrDefault(l => l.LeaseId == leaseId);

                if (lease == null)
                {
                    return Json(new { success = false, error = "Lease not found or not active" });
                }

                var invoice = await _invoiceService.CreateMonthlyInvoiceAsync(lease, billingMonth);

                if (invoice == null)
                {
                    return Json(new
                    {
                        success = false,
                        error = "Invoice already exists for this month or lease is not active"
                    });
                }

                var result = new
                {
                    success = true,
                    invoice = new
                    {
                        invoiceId = invoice.InvoiceId,
                        invoiceNumber = invoice.InvoiceNumber,
                        billingMonth = invoice.BillingMonthDisplay,
                        amount = invoice.TotalAmount,
                        dueDate = invoice.DueDate.ToString("yyyy-MM-dd"),
                        status = invoice.Status
                    },
                    emailSent = false,
                    emailError = (string?)null
                };

                if (sendEmail)
                {
                    var invoiceEntity = new DAL.Entities.Invoice
                    {
                        InvoiceId = invoice.InvoiceId,
                        InvoiceNumber = invoice.InvoiceNumber,
                        IssueDate = invoice.IssueDate,
                        DueDate = invoice.DueDate,
                        TotalAmount = invoice.TotalAmount,
                        BillingMonth = invoice.BillingMonth
                    };

                    try
                    {
                        await _emailService.SendInvoiceCreatedToTenantAsync(invoiceEntity, lease);
                        await _emailService.SendInvoiceCreatedToLandlordAsync(invoiceEntity, lease);
                        result = result with { emailSent = true };
                    }
                    catch (Exception emailEx)
                    {
                        result = result with { emailError = emailEx.Message };
                    }
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
