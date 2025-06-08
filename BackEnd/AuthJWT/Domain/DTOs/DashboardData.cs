namespace AuthJWT.Domain.DTOs
{
    public class DashboardData
    {
        public int TotalRoom { get; set; }
        public int TotalBooking { get; set; }
        public List<int> MonthlyBooking { get; set; }
        public List<int> MonthlyEarnings { get; set; }
        public decimal TotalEarnings { get; set; }
        public Dictionary<string, int> BookingStatusCounts { get; set; }
        public List<RecentBookingDto> RecentBookings { get; set; }
    }

    public class RecentBookingDto
    {
        public Guid Id { get; set; }
        public string GuestName { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public string HotelName { get; set; }
    }
}