using AuthJWT.Domain.DTOs;

namespace AuthJWT.Services.Interfaces
{
    public interface IHotelConvenienceService
    {
        Task<IEnumerable<HotelConvenienceDto>> GetAllHotelConveniencesAsync(Guid hotelId);
        Task<HotelConvenienceDto> GetHotelConvenienceByIdAsync(Guid hotelId, Guid convenienceId);
        Task<HotelConvenienceDto> CreateHotelConvenienceAsync(HotelConvenienceDto hotelConvenience);
        Task<HotelConvenienceDto> UpdateHotelConvenienceAsync(Guid hotelId, Guid convenienceId, HotelConvenienceDto hotelConvenience);
        Task<bool> DeleteHotelConvenienceAsync(Guid hotelId, Guid convenienceId);
        Task CreateHotelConveniencesAsync(Guid hotelId, IEnumerable<HotelConvenienceDto> hotelConveniences);


    }
}