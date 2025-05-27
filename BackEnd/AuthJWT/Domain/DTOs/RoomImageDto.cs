namespace AuthJWT.Domain.DTOs
{
    public class RoomImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public string ImageType { get; set; } 

    }
    public class RoomImageCreateDto
    {
        public Guid RoomId { get; set; }
        public Guid ImageUrl { get; set; }
        public string ImageType { get; set; } 
    }
    public class RoomImageUpdateDto
    {
        public Guid Id { get; set; }
        public Guid ImageUrl { get; set; }
        public string ImageType { get; set; } 
    }
}