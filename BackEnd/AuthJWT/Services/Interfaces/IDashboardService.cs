using AuthJWT.Domain.DTOs;

namespace AuthJWT.Services.Interfaces
{
    public interface IDashboardService
    {   
        Task<DashboardData> GetDashboardDataAsync(Guid hotelId);
    }
}