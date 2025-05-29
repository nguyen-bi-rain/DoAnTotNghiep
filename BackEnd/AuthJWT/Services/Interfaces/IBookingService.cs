using AuthJWT.Domain.DTOs;

namespace AuthJWT.Services.Interfaces
{
    public interface IBookingService
    {
        Task AddBookingAsync(BookingCreateDto bookingDtos);
        Task<IEnumerable<BookingResponse>> GetBookingsByHotelIdAsync(Guid hotelId, string? status);
        Task<IEnumerable<BookingResponse>> GetBookingsByUserIdAsync(string userId, string? status);
        Task UpdatStatusBookingAsync(Guid bookingId,string status,string? cancellationReason);
        Task UpdateBookignAsync(Guid bookindId, BookingCreateDto bookingDtos);
        Task<bool> VerifyBookingAsync(Guid bookingId, string userId);
    }
}