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
