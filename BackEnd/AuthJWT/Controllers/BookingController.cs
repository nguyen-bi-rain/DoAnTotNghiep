using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using AuthJWT.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthJWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IInvoiceService _invoiceService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "User,HotelOwner")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto bookingCreateDto)
        {
            try
            {
                await _bookingService.AddBookingAsync(bookingCreateDto);
                return Ok(new { message = "Booking created successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("hotel/{hotelId}")]
        [Authorize(Roles = "HotelOwner,Admin")]
        public async Task<IActionResult> GetBookingsByHotelId(Guid hotelId, string? status)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByHotelIdAsync(hotelId, status);
                return Ok(ApiResponse<IEnumerable<BookingResponse>>.Success(bookings));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update-status/{bookingId}")]
        [Authorize(Roles = "User,HotelOwner,Admin")]
        public async Task<IActionResult> UpdateBookingStatus(Guid bookingId, string status, string? cancellationReason)
        {
            try
            {
                await _bookingService.UpdatStatusBookingAsync(bookingId, status, cancellationReason);
                return Ok(new { message = "Booking status updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "User,HotelOwner,Admin")]
        public async Task<IActionResult> GetBookingsByUserId(string userId, string? status)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByUserIdAsync(userId, status);
                return Ok(ApiResponse<IEnumerable<BookingResponse>>.Success(bookings));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("verify/{bookingId}")]
        [Authorize(Roles = "User,HotelOwner,Admin")]
        public async Task<IActionResult> VerifyBooking(Guid bookingId, string userId)
        {
            try
            {
                var isVerified = await _bookingService.VerifyBookingAsync(bookingId, userId);
                return Ok(new { isVerified });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}