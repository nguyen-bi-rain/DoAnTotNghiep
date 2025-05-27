using AuthJWT.Domain.Entities.Common;
using Microsoft.AspNetCore.Identity;

namespace AuthJWT.Domain.Entities.Security
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Gender { get; set; }
        public string? Location { get; set; }
        public string? IdentifyNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool isActive { get; set; } = true;
        public string? Avatar { get; set; }
        public bool isHotelOwner { get; set; } = false;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
