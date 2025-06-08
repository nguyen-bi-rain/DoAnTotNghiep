using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Exceptions;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Services.Implements
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISendEmailService _sendEmailService;
        private readonly IRoomService _roomService;
        private readonly IS3Service _s3Service;

        public BookingService(IS3Service s3Service, IUnitOfWork unitOfWork, IMapper mapper, ISendEmailService sendEmailService, IRoomService roomService)
        {
            _s3Service = s3Service;
            _roomService = roomService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sendEmailService = sendEmailService;
        }

        public async Task<Booking> AddBookingAsync(BookingCreateDto bookingDtos)
        {
            // Map booking and its rooms
            var booking = _mapper.Map<Booking>(bookingDtos);
            booking.BookingRooms = new List<BookingRoom>();

            foreach (var bookingRoomDto in bookingDtos.BookingRooms)
            {
                var bookingRoomEntity = _mapper.Map<BookingRoom>(bookingRoomDto);
                booking.BookingRooms.Add(bookingRoomEntity);
            }

            // Add booking (with rooms) to the context
            var res = await _unitOfWork.BookingRepository.AddAsync(booking);

            // Update room quantities
            foreach (var bookingRoom in booking.BookingRooms)
            {
                await _roomService.UpdateQuantityOfRoomAsync(bookingRoom.RoomId, -bookingRoom.Quantity);
            }

            // Save all changes at once
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                throw new DatabaseBadRequestException("Failed to create booking.");
            }
            return res;
        }

        public async Task<PaginateList<BookingResponse>> GetBookingsByHotelIdAsync(Guid hotelId,string? status, int pageNumber, int pageSize)
        {
            var bookings = await _unitOfWork.BookingRepository.GetQuery()
                .Include(x => x.BookingRooms)
                .ThenInclude(x => x.Room)
                .Include(x => x.Hotel)
                .ThenInclude(x => x.HotelImages)
                .Include(x => x.User)
                .Where(x => x.HotelId == hotelId)
                .ToListAsync();

            if (status != null)
            {
                bookings = bookings.Where(x => x.Status == status).ToList();
            }

            if (bookings == null || !bookings.Any())
            {
                throw new Exception($"No bookings found for hotel with Id : {hotelId}");
            }

            var bookingResponses = new List<BookingResponse>();
            foreach (var booking in bookings)
            {
                var hotelImageUrl = booking.Hotel?.HotelImages
                    .FirstOrDefault(x => x.ImageType == "Thumbnail")?.ImageUrl.ToString() ?? string.Empty;

                // Get image from S3 if exists
                if (!string.IsNullOrEmpty(hotelImageUrl))
                {
                    hotelImageUrl = await _s3Service.GetFileUrlAsync(hotelImageUrl);
                }

                var bookingResponse = new BookingResponse
                {
                    Id = booking.Id,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    Adults = booking.Adults,
                    Children = booking.Children,
                    Status = booking.Status,
                    TotalPrice = booking.TotalPrice,
                    CancellationReason = booking.CancellationReason,
                    BookingRooms = _mapper.Map<List<BookingRoomResponse>>(booking.BookingRooms),
                    HotelName = booking.Hotel?.Name ?? string.Empty,
                    HotelImage = hotelImageUrl
                };

                bookingResponses.Add(bookingResponse);
            }


            return PaginateList<BookingResponse>.Create(bookingResponses, pageNumber, pageSize);
        }

        public async Task<PaginateList<BookingResponse>> GetBookingsByUserIdAsync(string userId,string? status, int pageNumber, int pageSize)
        {
            var bookings = await _unitOfWork.BookingRepository.GetQuery()
                 .Include(x => x.BookingRooms)
                 .ThenInclude(x => x.Room)
                 .Include(x => x.Hotel)
                 .ThenInclude(x => x.HotelImages)
                 .Include(x => x.User)
                 .Where(x => x.UserId == userId)
                 .ToListAsync();

            if (status != null)
            {
                bookings = bookings.Where(x => x.Status == status).ToList();
            }
            if (bookings == null || !bookings.Any())
                {
                    throw new Exception($"No bookings found for hotel with Id : {userId}");
                }

            var bookingResponses = new List<BookingResponse>();
            foreach (var booking in bookings)
            {
                var hotelImageUrl = booking.Hotel?.HotelImages
                    .FirstOrDefault(x => x.ImageType == "Thumbnail")?.ImageUrl.ToString() ?? string.Empty;

                // Get image from S3 if exists
                if (!string.IsNullOrEmpty(hotelImageUrl))
                {
                    hotelImageUrl = await _s3Service.GetFileUrlAsync(hotelImageUrl);
                }

                var bookingResponse = new BookingResponse
                {
                    Id = booking.Id,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    Adults = booking.Adults,
                    Children = booking.Children,
                    Status = booking.Status,
                    TotalPrice = booking.TotalPrice,
                    CancellationReason = booking.CancellationReason,
                    BookingRooms = _mapper.Map<List<BookingRoomResponse>>(booking.BookingRooms),
                    HotelName = booking.Hotel?.Name ?? string.Empty,
                    HotelImage = hotelImageUrl
                };

                bookingResponses.Add(bookingResponse);
            }
            return PaginateList<BookingResponse>.Create(bookingResponses, pageNumber, pageSize);

        }

        public async Task UpdateBookignAsync(Guid bookindId, BookingCreateDto bookingDtos)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookindId);
            if (booking == null)
            {
                throw new Exception($"Booking with Id : {bookindId} not found");
            }

            // Restore room quantities for the current booking rooms
            foreach (var existingRoom in booking.BookingRooms)
            {
                await _roomService.UpdateQuantityOfRoomAsync(existingRoom.RoomId, existingRoom.Quantity);
            }

            // Update booking details
            booking.CheckInDate = bookingDtos.CheckInDate;
            booking.CheckOutDate = bookingDtos.CheckOutDate;
            booking.Adults = bookingDtos.Adults;
            booking.Children = bookingDtos.Children;
            booking.Status = "Pending";
            booking.TotalPrice = bookingDtos.TotalPrice;
            booking.CancellationReason = bookingDtos.CancellationReason;
            booking.HotelId = bookingDtos.HotelId;
            booking.UserId = bookingDtos.UserId;

            // Clear existing booking rooms
            booking.BookingRooms.Clear();

            // Add updated booking rooms
            foreach (var bookingRoomDto in bookingDtos.BookingRooms)
            {
                var bookingRoom = _mapper.Map<BookingRoom>(bookingRoomDto);
                bookingRoom.BookingId = booking.Id;
                booking.BookingRooms.Add(bookingRoom);

                // Update room quantities for the new booking rooms
                await _roomService.UpdateQuantityOfRoomAsync(bookingRoom.RoomId, -bookingRoomDto.Quantity);
            }

            _unitOfWork.BookingRepository.Update(booking);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                throw new Exception("Failed to update booking.");
            }

        }

        public async Task UpdatStatusBookingAsync(Guid bookingId, string status, string? cancellationReason)
        {
            var booking = await _unitOfWork.BookingRepository.GetQuery(x => x.Id == bookingId)
                .Include(x => x.User)
                .Include(x => x.BookingRooms)
                .FirstOrDefaultAsync();
            if (booking == null)
            {
                throw new Exception($"Booking with Id : {bookingId} not found");
            }

            booking.Status = status;
            if (cancellationReason != null && status == "cancelled")
            {
                booking.CancellationReason = cancellationReason;
            }

            if (status == "cancelled")
            {
                foreach (var bookingRoom in booking.BookingRooms)
                {
                    await _roomService.UpdateQuantityOfRoomAsync(bookingRoom.RoomId, bookingRoom.Quantity);
                }
            }
            if (status == "completed")
            {
                foreach (var bookingRoom in booking.BookingRooms)
                {
                    await _roomService.UpdateQuantityOfRoomAsync(bookingRoom.RoomId, bookingRoom.Quantity);
                }
            }

            _unitOfWork.BookingRepository.Update(booking);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                throw new Exception("Failed to update booking status.");
            }
            var email = new MailRequest
            {
                ToEmail = booking.User.Email,
                Subject = "Booking Status Update",
                Body = $@"
                    <html>
                        <body>
                            <h2>Booking Status Update</h2>
                            <p>Dear {booking.User.FirstName},</p>
                            <p>Your booking status has been updated to <strong>{status}</strong>.</p>
                            <p>Booking Id: {booking.Id}</p>
                            {(!string.IsNullOrEmpty(cancellationReason) ? $"<p>Reason for cancellation: {cancellationReason}</p>" : "")}
                            <p>Thank you for using our service.</p>
                            <p>Best regards,</p>
                            <p>Your Hotel Team</p>
                        </body>
                    </html>"
            };
            await _sendEmailService.SendEmailAsync(email);
        }

        public Task<bool> VerifyBookingAsync(Guid bookingId, string userId)
        {
            return _unitOfWork.BookingRepository.GetQuery(x => x.Id == bookingId && x.UserId == userId)
                .AnyAsync();
                
        }
    }
}