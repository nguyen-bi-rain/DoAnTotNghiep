using Microsoft.AspNetCore.Mvc;
using AuthJWT.Domain.DTOs.HotelDto;
using AuthJWT.Helpers;
using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using AuthJWT.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AuthJWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IHotelService _hotelService;
        private readonly IRatingService _ratingService;
        private readonly IRoomService _roomService;
        public HotelController(IHotelService hotelService, IRatingService ratingService, IRoomService roomService, IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
            _roomService = roomService;
            _hotelService = hotelService;
            _ratingService = ratingService;
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetAllHotels()
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var hotels = await _hotelService.GetAllHotelsAsync();
                return Ok(ApiResponse<IEnumerable<HotelDto>>.Success(hotels));
            });
        }

        [HttpGet("{id}")]
        // [Authorize(Roles = "Admin,HotelOwner,User")]
        public async Task<IActionResult> GetHotelById(Guid id)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var hotel = await _hotelService.GetHotelByIdAsync(id);
                return Ok(ApiResponse<HotelDetailResponse>.Success(hotel));
            });
        }

        [HttpPost]
        [Authorize(Roles = "HotelOwner")]
        [RequestSizeLimit(10 * 1024 * 1024)] // Limit to 10 MB
        public async Task<IActionResult> CreateHotel([FromForm] HotelCreateDto hotelDto, [FromForm] List<IFormFile>? files)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                await _hotelService.CreateHotelAsync(hotelDto, files);
                return Ok(ApiResponse<string>.Success("hehe", "Hotel created successfully"));
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "HotelOwner")]
        public async Task<IActionResult> UpdateHotel([FromForm] HotelUpdateDto hotelDto, [FromForm] List<IFormFile>? files)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var updatedHotel = await _hotelService.UpdateHotelAsync(hotelDto, files);
                return Ok(ApiResponse<HotelUpdateDto>.Success(updatedHotel, "Hotel updated successfully"));
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "HotelOwner,Admin")]
        public async Task<IActionResult> DeleteHotel(Guid id)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                await _hotelService.DeleteHotelAsync(id);
                return Ok(ApiResponse<string>.Success("", "Hotel deleted successfully"));
            });
        }

        [HttpGet("get-ratings/{hotelId}")]
        [Authorize(Roles = "User,HotelOwner,Admin")]
        public async Task<IActionResult> GetRatingsByHotelId(Guid hotelId)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var ratings = await _ratingService.GetRatingsByHotelIdAsync(hotelId);
                return Ok(ApiResponse<PaginateList<RatingResponse>>.Success(ratings));
            });
        }
        [HttpGet("get-rooms/{hotelId}")]
        [Authorize(Roles = "User,HotelOwner,Admin")]
        public async Task<IActionResult> GetRoomsByHotelId(Guid hotelId, int pageIndex = 1, int pageSize = 10)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var rooms = await _roomService.GetRoomsByHotelIdAsync(hotelId, pageIndex, pageSize);
                return Ok(ApiResponse<PaginateList<RoomDto>>.Success(rooms));
            });
        }
        [HttpGet("search")]
        [Authorize(Roles = "User,HotelOwner,Admin")]
        public Task<IActionResult> SearchHotel(DateTime fromDate, DateTime endDate, int numberGuest, string location, int pageIndex = 1, int pageSize = 10, int sort = 1)
        {
            return HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var hotels = await _hotelService.SearchAvailableRoom(fromDate, endDate, numberGuest, location, pageIndex, pageSize, sort);
                return Ok(ApiResponse<PaginateList<HotelResponse>>.Success(hotels));
            });
        }
        [HttpGet("recommendations")]
        [Authorize]
        public async Task<IActionResult> GetRecommendedRooms(DateTime fromDate = default, DateTime endDate = default, int numberGuest = 1, string location = null, int page = 1, int pageSize = 10,int sortBy = 0)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var recommendedRooms = await _recommendationService.GetRecommendedRooms(userId, fromDate, endDate, numberGuest, location, page, pageSize, sortBy);
                return Ok(ApiResponse<PaginateList<HotelResponse>>.Success(recommendedRooms));
            });
        }

    }
}
