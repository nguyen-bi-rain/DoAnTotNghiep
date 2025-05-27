namespace AuthJWT.Domain.Entities.Common;

public class HotelConvenience : BaseEntity
{
    public Guid HotelId { get; set; } 
    public Guid ConvenienceId { get; set; } 
    public virtual Hotel Hotel { get; set; }
    public virtual Convenience Convenience { get; set; }
}
