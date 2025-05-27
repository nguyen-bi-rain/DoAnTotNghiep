using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs.HotelDto;

namespace AuthJWT.Services.Interfaces
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelResponse>> GetAllHotelsResponseAsync();
        Task<IEnumerable<HotelDto>> GetAllHotelsAsync();
        Task<HotelDetailResponse> GetHotelByIdAsync(Guid id);
        Task CreateHotelAsync(HotelCreateDto hotel, List<IFormFile>? files);
        Task<HotelUpdateDto> UpdateHotelAsync(HotelUpdateDto hotel,List<IFormFile>? files);
        Task<bool> DeleteHotelAsync(Guid id);
        Task<PaginateList<HotelResponse>> SearchAvailableRoom(DateTime fromDate, DateTime endDate, int numberGuest, string location, int pageNumber = 1, int pageSize = 10, int sort = 1);
        string GetHotelIdByUserId(string Userid);
    }
}
