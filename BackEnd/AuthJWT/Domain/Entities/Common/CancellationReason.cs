using System.ComponentModel.DataAnnotations.Schema;

namespace AuthJWT.Domain.Entities.Common
{
    public class CancellationReason : BaseEntity
    {
        public Guid BookingId { get; set; }
        public string CancellationDate  { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal  RefundAmount  { get; set; }
        public string RefundStatus  { get; set; }
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }
    }
}