using System.ComponentModel.DataAnnotations.Schema;

namespace AuthJWT.Domain.Entities.Common
{
    public class Policy : BaseEntity
    {
        public Guid HotelId { get; set; }
        public string PolicyType { get; set; }
        public string PolicyDetails { get; set; }
        [ForeignKey("HotelId")]
        public virtual Hotel Hotel { get; set; }
    }
}