using AuthJWT.Domain.DTOs;

namespace AuthJWT.Services.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAllLocationsAsync();
    }
}