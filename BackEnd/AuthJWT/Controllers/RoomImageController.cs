using AuthJWT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AuthJWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomImageController : ControllerBase
    {
        private readonly IRoomImageService _roomImageService;

        public RoomImageController(IRoomImageService roomImageService)
        {
            _roomImageService = roomImageService;
        }

        [HttpDelete("deleteImage/{imageId}")]
        public async Task<IActionResult> DeleteImage(Guid imageId)
        {
            try
            {
                var result = await _roomImageService.DeleteRoomImageAsync(imageId);
                if (result)
                {
                    return Ok(new { message = "Image deleted successfully." });
                }
                else
                {
                    return NotFound(new { message = "Image not found." });
                }
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Image not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}