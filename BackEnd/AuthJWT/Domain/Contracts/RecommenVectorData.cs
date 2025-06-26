namespace AuthJWT.Domain.Contracts
{
    public class RecommenVectorData
    {
        public string HotelName { get; set; }
        public Guid RoomId { get; set; }
        public double RoomPrice { get; set; }
        public int RoomNumber { get; set; }
        public Guid RoomTypeId { get; set; }
        public int Capacity { get; set; }
        public int BedCount { get; set; }
        public string ViewType { get; set; }
        public List<string> Convenviences { get; set; }
        public double RatingValue { get; set; }
        public int BookingCount { get; set; }
    }
    public class RecommendationResult
    {
        public RecommenVectorData HotelRoom { get; set; }
        public double SimilarityScore { get; set; }
    }
}