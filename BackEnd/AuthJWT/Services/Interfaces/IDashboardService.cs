using AuthJWT.Domain.DTOs;

namespace AuthJWT.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardData> GetHotelOwnerDashboardDataAsync(string userId, int year = 0);
    }
}