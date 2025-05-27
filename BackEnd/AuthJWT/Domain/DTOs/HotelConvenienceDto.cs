namespace AuthJWT.Domain.DTOs
{
    public class HotelConvenienceDto
    {
        public Guid HotelId { get; set; } // Foreign key to Hotel entity
        public Guid ConvenienceId { get; set; } // Foreign key to Convenience entity
    }
    public class HotelConvenienceReponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}