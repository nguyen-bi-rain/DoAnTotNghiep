using System.ComponentModel.DataAnnotations;
using AuthJWT.Domain.Enums;


namespace AuthJWT.Domain.Entities.Common;

public class Convenience : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    [Required]
    [StringLength(255)]
    public string? Description { get; set; }
    public ConvenienceType Type { get; set; }
    public virtual ICollection<RoomConvenience> Rooms { get; set; } = new List<RoomConvenience>();
    public virtual ICollection<HotelConvenience> Hotels { get; set; } = new List<HotelConvenience>();
}