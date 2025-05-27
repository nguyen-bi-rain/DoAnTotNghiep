using System.ComponentModel.DataAnnotations.Schema;

namespace AuthJWT.Domain.Entities.Common
{
    public class BookingRoom : BaseEntity
    {
        public Guid BookingId { get; set; }
        public Guid RoomId { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }

    }
}