using AuthJWT.Domain.DTOs;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;

namespace AuthJWT.Services.Implements
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public Task<DashboardData> GetDashboardDataAsync(Guid hotelId)
        {
            throw new NotImplementedException();
        }
    }
}