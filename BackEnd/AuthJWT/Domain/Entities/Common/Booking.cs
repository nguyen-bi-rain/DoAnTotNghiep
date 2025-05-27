using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuthJWT.Domain.Entities.Security;

namespace AuthJWT.Domain.Entities.Common;

public class Booking : BaseEntity
{
    [Required]
    [StringLength(450)]
    public string UserId { get; set; }
    [Required]
    public Guid HotelId { get; set; }
    [Required]
    public DateTime CheckInDate { get; set; }
    [Required]
    public DateTime CheckOutDate { get; set; }
    [Required]
    public int Adults { get; set; } = 1;
    [Required]
    public int Children { get; set; } = 0;
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Pending";
    public decimal TotalPrice { get; set; }
    public string? CancellationReason { get; set; }
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }
    [ForeignKey("HotelId")]
    public virtual Hotel Hotel { get; set; }
    public virtual ICollection<BookingRoom> BookingRooms { get; set; }
}