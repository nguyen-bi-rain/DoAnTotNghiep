namespace AuthJWT.Domain.DTOs;

public class BookingResponse
{
    public Guid Id { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public string Status { get; set; } = "Pending";
    public decimal TotalPrice { get; set; }
    public string? CancellationReason { get; set; }
    public List<BookingRoomResponse> BookingRooms { get; set; }
    public string HotelName { get; set; }
    public string HotelImage { get; set; }

}
public class BookingCreateDto
{
    public Guid HotelId { get; set; }
    public string UserId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public string Status { get; set; } = "Pending";
    public decimal TotalPrice { get; set; }
    public string? CancellationReason { get; set; }
    public List<BookingRoomCreateDto> BookingRooms { get; set; }
}