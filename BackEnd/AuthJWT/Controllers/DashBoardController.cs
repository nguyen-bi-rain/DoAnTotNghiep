using System.Security.Claims;
using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.Enums;
using AuthJWT.Exceptions;
using AuthJWT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthJWT.Controllers
{
    [ApiController]
    [Route("api/")]
    public class DashBoardController(IRoomTypeService roomTypeService,
        IDashboardService dashboardService,
        ILocationService locationService,
        IConvenienceService convenienceService) : ControllerBase
    {
        private readonly IDashboardService _dashboardService = dashboardService;
        private readonly IConvenienceService _convenienceService = convenienceService;
        private readonly IRoomTypeService _roomTypeService = roomTypeService;
        private readonly ILocationService _locationService = locationService;

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
        [HttpGet("dashboard/hotel-owner")]
        public async Task<IActionResult> GetHotelOwnerDashboardData(string userId,int year = 0)
        {
            try
            {
                var dashboardData = await _dashboardService.GetHotelOwnerDashboardDataAsync(userId, year);

                return Ok(ApiResponse<DashboardData>.Success(dashboardData));
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Failure("An unexpected error occurred: " + ex.Message));
            }
        }
    }
}