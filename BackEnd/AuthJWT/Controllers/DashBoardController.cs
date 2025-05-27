using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.Enums;
using AuthJWT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthJWT.Controllers
{
    [ApiController]
    [Route("api/")]
    public class DashBoardController : ControllerBase
    {
        private readonly IConvenienceService _convenienceService;
        private readonly IRoomTypeService _roomTypeService;
        private readonly ILocationService _locationService;

        public DashBoardController(IRoomTypeService roomTypeService,
            ILocationService locationService,
            IConvenienceService convenienceService)
        {
            _convenienceService = convenienceService;
            _locationService = locationService;
            _roomTypeService = roomTypeService;
        }

        [HttpGet("roomtypes")]
        public async Task<IActionResult> GetRoomTypes()
        {
            var roomTypes = await _roomTypeService.GetAllRoomTypesAsync();
            return Ok(ApiResponse<IEnumerable<RoomTypeDto>>.Success(roomTypes));
        }

        [HttpGet("locations")]
        public async Task<IActionResult> GetLocations()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return Ok(ApiResponse<IEnumerable<LocationDto>>.Success(locations));
        }

        [HttpGet("conveniences")]
        public async Task<IActionResult> GetConveniences(ConvenienceType type)
        {
            var conveniences = await _convenienceService.GetAllConveniencesAsync(type);
            return Ok(ApiResponse<IEnumerable<ConvenienceDto>>.Success(conveniences));
        }
        
        
    }
}