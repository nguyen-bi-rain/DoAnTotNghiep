namespace AuthJWT.Domain.DTOs
{
    public class RoomConvenienceDto
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid ConvenienceId { get; set; }
    }
    
    public class RoomConvenienceCreateDto
    {
        public Guid RoomId { get; set; }
        public Guid ConvenienceId { get; set; }
    }
    public class RoomConvenienceUpdateDto
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid ConvenienceId { get; set; }
    }
}