namespace AuthJWT.Domain.DTOs
{
    public class BookingRoomDto
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid BookingId { get; set; }
        public int Quantity { get; set; }
    }
    public class BookingRoomCreateDto
    {
        public Guid RoomId { get; set; }
        public Guid BookingId { get; set; }
        public int Quantity { get; set; }
    }
    public class BookingRoomResponse
    {
        public RoomResponse Room { get; set; }
        public int Quantity { get; set; }
    }
}