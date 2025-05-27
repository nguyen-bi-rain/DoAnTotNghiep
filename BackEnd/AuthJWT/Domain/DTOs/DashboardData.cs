namespace AuthJWT.Domain.DTOs
{
    public class DashboardData
    {
        public int TotalRoom { get; set; }
        public int TotalBooking { get; set; }
        public List<int> MonthlyBooking { get; set; }
        public List<int> MonthlyEarnings { get; set; }
        public decimal TotalEarnings { get; set; }
    }
}