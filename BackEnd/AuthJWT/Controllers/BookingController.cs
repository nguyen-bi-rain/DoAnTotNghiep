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
        public BookingController(IBookingService bookingService, IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
            _bookingService = bookingService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "User,HotelOwner")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto bookingCreateDto)
        {
            try
            {
                var res = await _bookingService.AddBookingAsync(bookingCreateDto);
                return Ok(new { message = res.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("hotel/{hotelId}")]
        [Authorize(Roles = "HotelOwner,Admin")]
        public async Task<IActionResult> GetBookingsByHotelId(Guid hotelId, string? status, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByHotelIdAsync(hotelId, status, pageNumber, pageSize);
                return Ok(ApiResponse<PaginateList<BookingResponse>>.Success(bookings));
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
        public async Task<IActionResult> GetBookingsByUserId(string userId, string? status, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByUserIdAsync(userId, status, pageNumber, pageSize);
                return Ok(ApiResponse<PaginateList<BookingResponse>>.Success(bookings));
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
        [HttpPost("create-invoice")]
        [Authorize(Roles = "User,HotelOwner")]
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceCreateDto invoiceCreateDto)
        {
            try
            {
                await _invoiceService.AddInvoiceAsync(invoiceCreateDto);
                return Ok(new { message = "Invoice created successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("hotel-invoices/{hotelId}")]
        [Authorize(Roles = "HotelOwner,Admin")]
        [ProducesResponseType(typeof(InvoiceDto), 200)]
        public async Task<IActionResult> GetInvoicesByHotelId(Guid hotelId, int pageNumber = 1, int pageSize = 10, string? search = null, string? status = null)
        {
            try
            {
                var invoices = await _invoiceService.GetInvoicesByHotelIdAsync(hotelId, pageNumber, pageSize, search, status);
                return Ok(ApiResponse<PaginateList<InvoiceDto>>.Success(invoices));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("user-invoices/{userId}")]
        [Authorize(Roles = "User,HotelOwner,Admin")]
        [ProducesResponseType(typeof(InvoiceForUser), 200)]

        public async Task<IActionResult> GetInvoicesByUserId(string userId)
        {
            try
            {
                var invoices = await _invoiceService.GetInvoicesByUserIdAsync(userId);
                return Ok(ApiResponse<IEnumerable<InvoiceForUser>>.Success(invoices));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("update-invoice-status/{invoiceId}")]
        [Authorize(Roles = "User,HotelOwner,Admin")]
        public async Task<IActionResult> UpdateInvoiceStatus(Guid invoiceId, string status)
        {
            try
            {
                await _invoiceService.UpdateStatusInvoiceAsync(invoiceId, status);
                return Ok(new { message = "Invoice status updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}