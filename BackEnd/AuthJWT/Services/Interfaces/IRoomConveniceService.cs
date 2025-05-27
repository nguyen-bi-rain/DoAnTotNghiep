using AuthJWT.Domain.DTOs;

namespace AuthJWT.Services.Interfaces
{
    public interface IRoomConveniceService
    {
        Task<bool> AddRoomConveniceAsync(RoomConvenienceCreateDto request);
        Task<bool> RemoveRoomConveniceAsync(Guid id);
        Task<List<Guid>> GetRoomConvenicesAsync(Guid roomId);
        Task<bool> IsRoomConveniceExistAsync(Guid roomId, Guid convenienceId);
    }
}