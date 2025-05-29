using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using AuthJWT.Helpers;
using AuthJWT.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthJWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        [Authorize(Roles = "HotelOwner,Admin,User")]
        public async Task<IActionResult> GetAllRooms(Guid hotelId, int capacity,int pageIndex = 1, int pageSize = 10, string? search = null,DateTime? checkInDate = null, DateTime? checkOutDate = null)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var rooms = await _roomService.GetRoomsByHotelIdAsync(hotelId, capacity, pageIndex, pageSize, search, checkInDate, checkOutDate);
                return Ok(ApiResponse<PaginateList<RoomDto>>.Success(rooms));
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "HotelOwner,Admin,User")]
        public async Task<IActionResult> GetRoomById(Guid id)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {

                var room = await _roomService.GetRoomByIdAsync(id);
                if (room == null)
                {
                    return NotFound(ApiResponse<string>.Failure("Room not found"));
                }
                return Ok(ApiResponse<RoomDto>.Success(room));
            });
        }

        [HttpPost]
        [Authorize(Roles = "HotelOwner")]
        [RequestSizeLimit(10 * 1024 * 1024)] // Limit to 10 MB
        public async Task<IActionResult> CreateRoom([FromForm] RoomCreateDto roomDto, [FromForm] List<IFormFile>? files)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                if (roomDto == null)
                {
                    return BadRequest(ApiResponse<string>.Failure("Invalid room data"));
                }
                var createdRoom = await _roomService.CreateRoomAsync(roomDto, files);
                return Ok(ApiResponse<RoomCreateDto>.Success(createdRoom, "Room created successfully"));
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "HotelOwner")]
        public async Task<IActionResult> UpdateRoom([FromForm] RoomUpdateDto roomDto,[FromForm] List<IFormFile>? files, Guid id)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                if (id != roomDto.Id)
                {
                    return BadRequest(ApiResponse<string>.Failure("Room ID mismatch"));
                }

                var updatedRoom = await _roomService.UpdateRoomAsync(roomDto, files);
                if (updatedRoom == null)
                {
                    return NotFound(ApiResponse<string>.Failure("Room not found"));
                }

                return Ok(ApiResponse<RoomUpdateDto>.Success(updatedRoom, "Room updated successfully"));
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "HotelOwner,Admin")]
        public async Task<IActionResult> DeleteRoom(Guid id)
        {
            return await HandleRequestHelper.HandleRequestAsync(async () =>
            {
                var result = await _roomService.DeleteRoomAsync(id);
                if (!result)
                {
                    return NotFound(ApiResponse<string>.Failure("Room not found"));
                }

                return NoContent();
            });
        }

    }
}