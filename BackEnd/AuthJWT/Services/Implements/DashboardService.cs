using AuthJWT.Domain.DTOs;
using AuthJWT.Exceptions;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Services.Implements
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardData> GetHotelOwnerDashboardDataAsync(string userId, int year = 0)
        {
            // If year is not provided, use current year
            if (year == 0)
                year = DateTime.Now.Year;

            // Get all hotels owned by this user
            var hotels = await _unitOfWork.HotelRepository.GetQuery(h => h.OwnerId == userId).ToListAsync();

            if (!hotels.Any())
                throw new ResourceNotFoundException("No hotels found for this owner");

            var hotelIds = hotels.Select(h => h.Id).ToList();

            // Get all rooms for these hotels
            var rooms = await _unitOfWork.RoomRepository.GetQuery(r => hotelIds.Contains(r.HotelId)).ToListAsync();
            var totalRooms = rooms.Sum(r => r.NumberOfRooms);

            // Get all bookings for these hotels in the specified year
            var bookings = await _unitOfWork.BookingRepository.GetQuery(b =>
                    hotelIds.Contains(b.HotelId) &&
                    b.CreatedAt.Year == year)
                .Include(b => b.BookingRooms)
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var totalBookings = bookings.Count;
            var totalEarnings = bookings.Where(b => b.Status != "Cancelled").Sum(b => b.TotalPrice);

            // Calculate monthly data
            var monthlyBookings = new List<int>(new int[12]);
            var monthlyEarnings = new List<int>(new int[12]);

            foreach (var booking in bookings)
            {
                var month = booking.CreatedAt.Month - 1; 
                monthlyBookings[month]++;

                if (booking.Status != "Cancelled")
                {
                    monthlyEarnings[month] += (int)booking.TotalPrice;
                }
            }

            // Get booking status counts
            var bookingStatusCounts = bookings
                .GroupBy(b => b.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            // Get recent bookings (last 10)
            var recentBookings = bookings
                .Take(10)
                .Select(b => new RecentBookingDto
                {
                    Id = b.Id,
                    GuestName = $"{b.User.FirstName} {b.User.LastName}",
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    Status = b.Status,
                    TotalPrice = b.TotalPrice,
                    BookingDate = b.CreatedAt,
                    HotelName = hotels.FirstOrDefault(h => h.Id == b.HotelId)?.Name
                })
                .ToList();

            // Return the dashboard data
            return new DashboardData
            {
                TotalRoom = totalRooms,
                TotalBooking = totalBookings,
                TotalEarnings = totalEarnings,
                MonthlyBooking = monthlyBookings,
                MonthlyEarnings = monthlyEarnings,
                BookingStatusCounts = bookingStatusCounts,
                RecentBookings = recentBookings
            };
        }
    }
}