using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using AuthJWT.Helpers;
using AuthJWT.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace AuthJWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }


        [HttpGet("hotel/{id}")]
        [Authorize(Roles = "User,HotelOwner,Admin")]
        public async Task<IActionResult> GetRatingHotel(Guid id, int pageIndex = 1, int pageSize = 10, string searchString = null, int sortBy = 0, int orderby = 0)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var rating = await _ratingService.GetRatingsByHotelIdAsync(id, pageIndex, pageSize, searchString, sortBy, orderby);
                return Ok(ApiResponse<PaginateList<RatingResponse>>.Success(rating));
            });
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateRating([FromBody] RatingCreateDto rating)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var createdRating = await _ratingService.CreateRatingAsync(rating);
                return Ok(ApiResponse<RatingCreateDto>.Success(createdRating, "Rating created successfully"));
            });
        }
        [HttpGet]
        [Authorize(Roles = "User,HotelOwner,Admin")]
        public async Task<IActionResult> GetAllRatings()
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var ratings = await _ratingService.GetAllRatingsAsync();
                return Ok(ApiResponse<IEnumerable<RatingDto>>.Success(ratings));
            });
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "User,HotelOwner,Admin")]
        public async Task<IActionResult> GetRatingById(Guid id)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var rating = await _ratingService.GetRatingByIdAsync(id);
                return Ok(ApiResponse<RatingDto>.Success(rating));
            });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "User,HotelOwner,Admin")]
        public async Task<IActionResult> DeleteRating(Guid id)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var isDeleted = await _ratingService.DeleteRatingAsync(id);
                return isDeleted
                    ? Ok(ApiResponse<string>.Success("Rating deleted successfully"))
                    : NotFound(ApiResponse<string>.Failure("Rating not found"));
            });
        }
    }
}