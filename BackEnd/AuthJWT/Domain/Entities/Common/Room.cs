using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthJWT.Domain.Entities.Common;

public class Room : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string RoomName { get; set; }
    [Required]
    [StringLength(20)]
    public Guid HotelId { get; set; }
    [Required]
    [StringLength(20)]
    public Guid RoomTypeId { get; set; }
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Available";
    [Required]
    public string ViewType { get; set; }
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PricePerNight { get; set; }
    public string Description { get; set; }
    public int Capacity { get; set; }
    public int NumberOfBeds { get; set; }
    public int NumberOfRooms { get; set; }
    public int AvailableRooms { get; set; }
    public string BedType { get; set; }
    [ForeignKey("HotelId")]
    public Hotel Hotel { get; set; }
    [ForeignKey("RoomTypeId")]
    public RoomType RoomType { get; set; }
    public virtual ICollection<RoomConvenience> Conveniences { get; set; } = new List<RoomConvenience>();
    public virtual ICollection<BookingRoom> Bookings { get; set; } = new List<BookingRoom>();
    public virtual ICollection<RoomImage> RoomImages { get; set; } = new List<RoomImage>();

}