namespace AuthJWT.Domain.DTOs
{
    public class HotelImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public required string ImageType { get; set; }
    }
    
    public class HotelImageCreateDto
    {
        public Guid HotelId { get; set; }
        public Guid ImageUrl { get; set; }
        public required string ImageType { get; set; }
    }
}