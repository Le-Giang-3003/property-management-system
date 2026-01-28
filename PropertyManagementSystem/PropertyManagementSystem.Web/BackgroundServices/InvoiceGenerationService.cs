using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.Web.BackgroundServices
{
    public class InvoiceGenerationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InvoiceGenerationService> _logger;

        public InvoiceGenerationService(
            IServiceProvider serviceProvider,
            ILogger<InvoiceGenerationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Invoice Generation Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Calculate time until next midnight (00:00)
                    var now = DateTime.Now;
                    var nextMidnight = now.Date.AddDays(1);
                    var delay = nextMidnight - now;

                    _logger.LogInformation("Next invoice generation scheduled at: {NextRun}", nextMidnight);

                    // Wait until midnight
                    await Task.Delay(delay, stoppingToken);

                    // Run the invoice generation
                    await GenerateInvoicesAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Service is stopping
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in Invoice Generation Service");
                    // Wait 1 hour before retrying if there's an error
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }

            _logger.LogInformation("Invoice Generation Service is stopping.");
        }

        private async Task GenerateInvoicesAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting automatic invoice generation at {Time}", DateTime.UtcNow);

            using var scope = _serviceProvider.CreateScope();

            var leaseRepository = scope.ServiceProvider.GetRequiredService<ILeaseRepository>();
            var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            try
            {
                // First, update any overdue invoices
                await invoiceService.UpdateOverdueInvoicesAsync();
                _logger.LogInformation("Overdue invoices updated");

                // Get all active leases
                var activeLeases = await leaseRepository.GetAllActiveLeasesAsync();
                var today = DateTime.UtcNow.Date;
                var currentMonth = new DateTime(today.Year, today.Month, 1);

                // Only generate monthly invoices on the 1st day of each month
                if (today.Day != 1)
                {
                    _logger.LogInformation("Today is not the 1st. Skipping monthly invoice generation.");
                    return;
                }

                int invoicesCreated = 0;
                int invoicesSkipped = 0;

                foreach (var lease in activeLeases)
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;

                    try
                    {
                        // Check if lease is still within its period
                        if (today < lease.StartDate || today > lease.EndDate)
                        {
                            continue;
                        }

                        // Create invoice for current month
                        var invoice = await invoiceService.CreateMonthlyInvoiceAsync(lease, currentMonth);

                        if (invoice != null)
                        {
                            invoicesCreated++;
                            _logger.LogInformation(
                                "Created invoice {InvoiceNumber} for lease {LeaseId}, tenant {TenantName}, amount {Amount}",
                                invoice.InvoiceNumber,
                                lease.LeaseId,
                                lease.Tenant?.FullName ?? "Unknown",
                                invoice.TotalAmount);

                            // Reload invoice entity for email (need full navigation properties)
                            var invoiceEntity = new DAL.Entities.Invoice
                            {
                                InvoiceId = invoice.InvoiceId,
                                InvoiceNumber = invoice.InvoiceNumber,
                                IssueDate = invoice.IssueDate,
                                DueDate = invoice.DueDate,
                                TotalAmount = invoice.TotalAmount,
                                BillingMonth = invoice.BillingMonth
                            };

                            // Send email notifications
                            try
                            {
                                await emailService.SendInvoiceCreatedToTenantAsync(invoiceEntity, lease);
                                _logger.LogInformation("Sent invoice email to tenant: {Email}", lease.Tenant?.Email);
                            }
                            catch (Exception emailEx)
                            {
                                _logger.LogWarning(emailEx, "Failed to send email to tenant {Email}", lease.Tenant?.Email);
                            }

                            try
                            {
                                await emailService.SendInvoiceCreatedToLandlordAsync(invoiceEntity, lease);
                                _logger.LogInformation("Sent invoice email to landlord: {Email}", lease.Property?.Landlord?.Email);
                            }
                            catch (Exception emailEx)
                            {
                                _logger.LogWarning(emailEx, "Failed to send email to landlord {Email}", lease.Property?.Landlord?.Email);
                            }
                        }
                        else
                        {
                            invoicesSkipped++;
                            _logger.LogDebug(
                                "Skipped invoice for lease {LeaseId} - already exists for this month",
                                lease.LeaseId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing lease {LeaseId}", lease.LeaseId);
                    }
                }

                _logger.LogInformation(
                    "Invoice generation completed. Created: {Created}, Skipped: {Skipped}",
                    invoicesCreated,
                    invoicesSkipped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during invoice generation");
                throw;
            }
        }

        /// <summary>
        /// Manual trigger for invoice generation (can be called from a controller for testing)
        /// </summary>
        public async Task TriggerGenerationAsync()
        {
            await GenerateInvoicesAsync(CancellationToken.None);
        }
    }
}
