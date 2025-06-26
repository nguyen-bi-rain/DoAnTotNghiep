using AuthJWT.Domain.Contracts;

namespace AuthJWT.Services.Interfaces
{
    public interface IRecommendationService
    {
        Task<List<RecommenVectorData>> CollectAllRoomData(DateTime fromDate = default, DateTime endDate = default, int numberGuest = 1, string location = null);
        Task<List<RecommenVectorData>> GetUserBookingHistory(string userId);
        Task<PaginateList<HotelResponse>> GetRecommendedRooms(
            string userId, 
            DateTime fromDate = default, 
            DateTime endDate = default, 
            int numberGuest = 1, 
            string location = null,
            int page = 1, 
            int pageSize = 10,int sortBy = 0);
    }
}