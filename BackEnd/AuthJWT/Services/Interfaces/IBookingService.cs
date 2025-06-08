using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.Entities.Common;

namespace AuthJWT.Services.Interfaces
{
    public interface IBookingService
    {
        Task<Booking> AddBookingAsync(BookingCreateDto bookingDtos);
        Task<PaginateList<BookingResponse>> GetBookingsByHotelIdAsync(Guid hotelId, string? status, int pageNumber, int pageSize);
        Task<PaginateList<BookingResponse>> GetBookingsByUserIdAsync(string userId, string? status,int pageNumber, int pageSize);
        Task UpdatStatusBookingAsync(Guid bookingId,string status,string? cancellationReason);
        Task UpdateBookignAsync(Guid bookindId, BookingCreateDto bookingDtos);
        Task<bool> VerifyBookingAsync(Guid bookingId, string userId);
    }
}