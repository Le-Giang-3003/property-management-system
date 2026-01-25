using PropertyManagementSystem.BLL.DTOs;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IDashboardService
    {
        Task<LandlordDashboardDto> GetLandlordDashboardAsync(int landlordId);
    }
}
