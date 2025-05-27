using AuthJWT.Domain.DTOs;

namespace AuthJWT.Services.Interfaces
{
    public interface IHotelImageService
    {
        Task CreateHotelImageAsync(HotelImageCreateDto hotelImageDto, IFormFile file);
        Task<IEnumerable<HotelImageDto>> GetImageBytHoelId(Guid hotelId);
        Task DeleteHotelImageAsync(Guid id);
        Task UpdatHotelImageAsync(Guid id, IFormFile file);


    }
}