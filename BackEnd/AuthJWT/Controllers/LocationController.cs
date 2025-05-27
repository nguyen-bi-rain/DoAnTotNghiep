using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using AuthJWT.Helpers;
using AuthJWT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthJWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLocations()
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var locations = await _locationService.GetAllLocationsAsync();
                return Ok(ApiResponse<IEnumerable<LocationDto>>.Success(locations));
            });
        }
    }
}