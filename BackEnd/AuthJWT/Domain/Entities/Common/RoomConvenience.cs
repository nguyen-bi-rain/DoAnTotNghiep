using System.ComponentModel.DataAnnotations;

namespace AuthJWT.Domain.Entities.Common;

public class RoomConvenience : BaseEntity
{
    [Required]
    public Guid RoomId { get; set; }
    [Required]
    public Guid ConvenienceId { get; set; }
    public Room Room { get; set; }
    public Convenience Convenience { get; set; }

}