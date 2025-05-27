using AuthJWT.Domain.DTOs;
using AuthJWT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthJWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelImageController : ControllerBase
    {
        private readonly IHotelImageService _hotelImageService;
        public HotelImageController(IHotelImageService hotelImageService)
        {
            _hotelImageService = hotelImageService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateHotelImage([FromForm] HotelImageCreateDto hotelImageDto, IFormFile file)
        {
            await _hotelImageService.CreateHotelImageAsync(hotelImageDto, file);
            return Ok(new { message = "Hotel image created successfully" });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotelImage(Guid id)
        {
            await _hotelImageService.DeleteHotelImageAsync(id);
            return Ok(new { message = "Hotel image deleted successfully" });
        }
    }
}