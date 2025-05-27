namespace AuthJWT.Services.Interfaces
{
    public interface IRoomImageService
    {
        Task AddRoomImageAsync(Guid roomId, List<IFormFile> images);
        Task<bool> DeleteRoomImageAsync(Guid imageId);

    }
}