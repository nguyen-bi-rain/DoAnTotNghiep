using AuthJWT.Domain.DTOs;

namespace AuthJWT.Services.Interfaces
{
    public interface IRoomTypeService
    {
        Task<RoomTypeCreateDto> CreateRoomTypeAsync(RoomTypeCreateDto roomType);
        Task<bool> DeleteRoomTypeAsync(Guid id);
        Task<IEnumerable<RoomTypeDto>> GetAllRoomTypesAsync();
        Task<RoomTypeDto> GetRoomTypeByIdAsync(Guid id);
        Task<RoomTypeDto> UpdateRoomTypeAsync(Guid id, RoomTypeUpdateDto roomType);
        
    }
}