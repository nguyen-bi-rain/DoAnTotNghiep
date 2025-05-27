namespace AuthJWT.Services.Interfaces
{
    public interface IHotelOwnerService
    {
        Task<string> GenerateHotelOwnerTokenAsync(string userId);
        Task<bool> VerifyHotelOwnerTokenAsync(string userId, string token);
    }
}