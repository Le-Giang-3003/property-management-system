using PropertyManagementSystem.BLL.DTOs;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TenantDashboardDto> GetTenantDashboardAsync(int tenantId)
        {
            // Get active leases count
            var leases = await _unitOfWork.Leases.GetByTenantIdAsync(tenantId);
            var activeLeases = leases.Where(l => l.Status == "Active").ToList();
            var activeLeasesCount = activeLeases.Count;

            // Get pending payments (unpaid invoices)
            var unpaidInvoices = await _unitOfWork.Invoices.GetAvailableInvoicesByTenantAsync(tenantId);
            var pendingPaymentsCount = unpaidInvoices.Count;

            // Get open maintenance requests
            var maintenanceStats = await _unitOfWork.MaintenanceRequests.GetRequestStatsByTenantAsync(tenantId);
            var openMaintenanceCount = maintenanceStats.GetValueOrDefault("Pending", 0) + 
                                       maintenanceStats.GetValueOrDefault("InProgress", 0);

            // Get saved properties count
            var favorites = await _unitOfWork.FavoriteProperties.GetFavoritesByUserIdAsync(tenantId);
            var savedPropertiesCount = favorites.Count();

            // Get upcoming payments (due in next 30 days)
            var upcomingPayments = unpaidInvoices
                .Where(i => i.DueDate <= DateTime.Now.AddDays(30))
                .OrderBy(i => i.DueDate)
                .Take(5)
                .Select(i => new UpcomingPaymentDto
                {
                    InvoiceId = i.InvoiceId,
                    PropertyName = i.Lease?.Property?.Name ?? "Property",
                    Description = $"Monthly Rent - {i.Lease?.Property?.Name ?? "Property"}",
                    Amount = i.RemainingAmount,
                    DueDate = i.DueDate
                })
                .ToList();

            // Get active leases with details
            var tenantLeases = activeLeases
                .OrderByDescending(l => l.StartDate)
                .Take(5)
                .Select(l => new TenantLeaseDto
                {
                    LeaseId = l.LeaseId,
                    PropertyId = l.PropertyId,
                    PropertyName = l.Property?.Name ?? "Property",
                    PropertyAddress = l.Property?.Address ?? "",
                    PropertyImageUrl = l.Property?.Images?.FirstOrDefault(i => i.IsThumbnail)?.ImageUrl 
                                      ?? l.Property?.Images?.FirstOrDefault()?.ImageUrl,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    MonthlyRent = l.MonthlyRent,
                    Status = l.Status
                })
                .ToList();

            // Build recent activities from various sources
            var activities = new List<ActivityItemDto>();

            // Add recent payments
            var recentPayments = await _unitOfWork.Payments.GetByTenantAsync(tenantId);
            activities.AddRange(recentPayments
                .OrderByDescending(p => p.PaymentDate)
                .Take(3)
                .Select(p => new ActivityItemDto
                {
                    Type = "Payment",
                    Title = "Payment Confirmed",
                    Description = $"Your rent payment of {p.Amount:N0} ₫ has been received",
                    Timestamp = p.PaymentDate
                }));

            // Add recent maintenance updates
            var maintenanceRequests = await _unitOfWork.MaintenanceRequests.GetByTenantIdAsync(tenantId);
            activities.AddRange(maintenanceRequests
                .Where(m => m.Status == "Completed" || m.Status == "InProgress")
                .OrderByDescending(m => m.CompletedDate ?? m.RequestDate)
                .Take(3)
                .Select(m => new ActivityItemDto
                {
                    Type = "Maintenance",
                    Title = m.Status == "Completed" ? "Maintenance Completed" : "Maintenance In Progress",
                    Description = $"{m.Title} - {m.Property?.Name ?? "Property"}",
                    Timestamp = m.CompletedDate ?? m.RequestDate
                }));

            // Sort all activities by timestamp
            var recentActivities = activities
                .OrderByDescending(a => a.Timestamp)
                .Take(5)
                .ToList();

            return new TenantDashboardDto
            {
                ActiveLeasesCount = activeLeasesCount,
                PendingPaymentsCount = pendingPaymentsCount,
                OpenMaintenanceCount = openMaintenanceCount,
                SavedPropertiesCount = savedPropertiesCount,
                UpcomingPayments = upcomingPayments,
                ActiveLeases = tenantLeases,
                RecentActivities = recentActivities
            };
        }

        public async Task<LandlordDashboardDto> GetLandlordDashboardAsync(int landlordId)
        {
            
            var total = await _unitOfWork.Properties.GetTotalByLandlordAsync(landlordId);
            var available = await _unitOfWork.Properties.GetCountByStatusAsync(landlordId, "Available");
            var rented = await _unitOfWork.Properties.GetCountByStatusAsync(landlordId, "Rented");
            var maintenance = await _unitOfWork.Properties.GetCountByStatusAsync(landlordId, "Maintenance");
            var revenue = await _unitOfWork.Properties.GetTotalMonthlyRevenueAsync(landlordId);
            var properties = await _unitOfWork.Properties.GetByLandlordWithDetailsAsync(landlordId, 6);
            var pendingMaintenance = await _unitOfWork.MaintenanceRequests.GetPendingCountByLandlordAsync(landlordId);
            var maintenanceRequests = await _unitOfWork.MaintenanceRequests.GetRecentByLandlordAsync(landlordId, 5);
            var pendingApplications = await _unitOfWork.RentalApplications.GetPendingCountByLandlordAsync(landlordId);
            var applications = await _unitOfWork.RentalApplications.GetRecentByLandlordAsync(landlordId, 5);
            var contracts = await _unitOfWork.Leases.GetExpiringByLandlordAsync(landlordId, 30);

            return new LandlordDashboardDto
            {
                TotalProperties = total,
                AvailableProperties = available,
                RentedProperties = rented,
                MaintenanceProperties = maintenance,
                TotalMonthlyRevenue = revenue,
                OccupancyRate = CalculateOccupancyRate(total, rented),
                PendingMaintenanceRequests = pendingMaintenance,
                RecentMaintenanceRequests = maintenanceRequests.Select(m => new MaintenanceRequestDto
                {
                    RequestId = m.RequestId,
                    Title = m.Title,
                    PropertyName = m.Property?.Name ?? "",
                    RequestDate = m.RequestDate,
                    Status = m.Status,
                    Priority = m.Priority
                }).ToList(),
                PendingApplicationsCount = pendingApplications,
                RecentApplications = applications.Select(a => new RentalApplicationDto
                {
                    ApplicationId = a.ApplicationId,
                    ApplicantName = a.Applicant?.FullName ?? "",
                    PropertyName = a.Property?.Name ?? "",
                    Status = a.Status
                }).ToList(),
                RecentProperties = properties.Select(p => new PropertyDto
                {
                    PropertyId = p.PropertyId,
                    Name = p.Name,
                    Address = p.Address,
                    Status = p.Status,
                    Bedrooms = p.Bedrooms,
                    Bathrooms = p.Bathrooms,
                    SquareFeet = p.SquareFeet,
                    RentAmount = p.RentAmount,
                    ThumbnailUrl = p.Images?.FirstOrDefault()?.ImageUrl
                }).ToList(),
                ExpiringContracts = contracts.Select(c => new LeaseDto
                {
                    LeaseId = c.LeaseId,
                    PropertyName = c.Property?.Name ?? "",
                    TenantName = c.Tenant?.FullName ?? "",
                }).ToList()
            };
        }

        private decimal CalculateOccupancyRate(int total, int rented)
        {
            if (total == 0) return 0;
            return Math.Round((decimal)rented / total * 100, 1);
        }
    }
}
