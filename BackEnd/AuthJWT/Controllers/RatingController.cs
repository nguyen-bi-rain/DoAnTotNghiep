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


        [HttpGet("{id}")]
        [Authorize(Roles = "User,HotelOwner,Admin")]
        public async Task<IActionResult> GetRatingHotel(Guid id, int pageIndex = 1, int pageSize = 10)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var rating = await _ratingService.GetRatingsByHotelIdAsync(id, pageIndex, pageSize);
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
    }
}