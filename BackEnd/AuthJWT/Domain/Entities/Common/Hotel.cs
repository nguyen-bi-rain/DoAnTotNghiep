using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuthJWT.Domain.Entities.Security;

namespace AuthJWT.Domain.Entities.Common
{
    public class Hotel : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        [Required]
        public Guid LocationId { get; set; }
        [Required]
        [StringLength(255)]
        public string Address { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }
        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(450)]
        public string OwnerId { get; set; }
        [Required]
        public bool IsApproved { get; set; } = false;
        // Navigation properties
        // [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public virtual ApplicationUser Owner { get; set; }
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public virtual ICollection<HotelImage> HotelImages { get; set; } = new List<HotelImage>();
        public virtual ICollection<HotelConvenience> HotelConveniences { get; set; } = new List<HotelConvenience>();
        public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();
    }
}