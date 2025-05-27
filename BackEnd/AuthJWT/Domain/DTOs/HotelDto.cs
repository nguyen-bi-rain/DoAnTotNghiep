using System.ComponentModel.DataAnnotations;
using AuthJWT.Domain.Entities.Common;

namespace AuthJWT.Domain.DTOs.HotelDto;

public class HotelDto
{
    public Guid Id { get; set; }
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }

    [Required]
    public Guid LocationId { get; set; }

    [Required]
    [StringLength(255)]
    public required string Address { get; set; }
    public required string Description { get; set; }
    [Required]
    [StringLength(15)]
    public required string PhoneNumber { get; set; }
    [Required]
    [StringLength(255)]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [StringLength(450)]
    public string OwnerId { get; set; }
    [Required]
    public bool IsApproved { get; set; } = false;
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<RatingDto> Ratings { get; set; } = new List<RatingDto>();
}

public class HotelCreateDto
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
    public ICollection<HotelConvenienceDto> Conveniences { get; set; } = new List<HotelConvenienceDto>();
    public ICollection<PolicyDto> Policies { get; set; }

}

public class HotelUpdateDto
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Guid LocationId { get; set; }
    [Required]
    public string Address { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;
    public ICollection<PolicyDto> Policies { get; set; } = new List<PolicyDto>();
    public ICollection<HotelConvenienceDto> Conveniences { get; set; } = new List<HotelConvenienceDto>();


}

public class HotelProfile
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string Description { get; set; }
    public required string PhoneNumber { get; set; }
    public string Email { get; set; }
    public bool IsApproved { get; set; } = false;
    public ICollection<Convenience> Conveniences { get; set; } = new List<Convenience>();
    public ICollection<HotelImageDto> HotelImages { get; set; } = new List<HotelImageDto>();

}
public class HotelDetailResponse
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string Description { get; set; }
    public required string PhoneNumber { get; set; }
    public string Email { get; set; }
    public bool IsApproved { get; set; }
    public ICollection<ConvenienceDto> Conveniences { get; set; } = new List<ConvenienceDto>();
    public ICollection<HotelImageDto> HotelImages { get; set; } = new List<HotelImageDto>();
    public ICollection<RoomDto> Rooms { get; set; } = new List<RoomDto>();
    public ICollection<PolicyDto> Policy { get; set; } = new List<PolicyDto>();

}