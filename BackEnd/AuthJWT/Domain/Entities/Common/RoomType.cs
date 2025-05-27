using System.ComponentModel.DataAnnotations;

namespace AuthJWT.Domain.Entities.Common;

public class RoomType : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string RoomTypeName { get; set; }
    [Required]
    [StringLength(255)]
    public string ShortDescription { get; set; }
    public virtual ICollection<Room> Rooms { get; set; }  = new List<Room>();
}