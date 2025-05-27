using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthJWT.Domain.Entities.Common
{
    public class Location : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string City { get; set; }
        [Required]
        [StringLength(100)]
        public string Country { get; set; }
        [Required]
        [Column(TypeName = "decimal(9,6)")]
        public decimal Latitude { get; set; }
        [Required]
        [Column(TypeName = "decimal(9,6)")]
        public decimal Longitude { get; set; }
        public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    }
}