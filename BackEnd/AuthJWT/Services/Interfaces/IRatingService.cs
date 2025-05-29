using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;

namespace AuthJWT.Services.Interfaces
{
    public interface IRatingService
    {
        Task<IEnumerable<RatingDto>> GetAllRatingsAsync();
        Task<RatingDto> GetRatingByIdAsync(Guid id);
        Task<RatingCreateDto> CreateRatingAsync(RatingCreateDto rating);
        Task<RatingUpdateDto> UpdateRatingAsync(RatingUpdateDto rating);
        Task<bool> DeleteRatingAsync(Guid id);
        Task<PaginateList<RatingResponse>> GetRatingsByHotelIdAsync(Guid hotelId,int pageIndex = 1, int pageSize = 10,string searchString = null,int sortBy = 0,int orderby = 0);
    }
}