using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;

namespace AuthJWT.Services.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
        Task<RoomDto> GetRoomByIdAsync(Guid id);
        Task<RoomCreateDto> CreateRoomAsync(RoomCreateDto room, List<IFormFile> files);
        Task<RoomUpdateDto> UpdateRoomAsync(RoomUpdateDto room,List<IFormFile>? files);
        Task<bool> DeleteRoomAsync(Guid id);
        Task<PaginateList<RoomDto>> GetRoomsByHotelIdAsync(Guid hotelId,int capacity, int pageIndex = 1, int pageSize = 10, string? search = null,DateTime? checkInDate = null, DateTime? checkOutDate = null);
        Task UpdateQuantityOfRoomAsync(Guid id, int quantity);
        Task<IEnumerable<RoomDto>> GetAllRoomsByHotelIdAsync(Guid hotelId);
    }

}