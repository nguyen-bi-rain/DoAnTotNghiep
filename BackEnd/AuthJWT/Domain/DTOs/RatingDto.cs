using System.ComponentModel.DataAnnotations;
using AuthJWT.Domain.Entities.Security;

namespace AuthJWT.Domain.DTOs;

public class RatingDto
{
    public Guid Id { get; set; }
    [Required]
    [StringLength(450)]
    public string UserId { get; set; }

    [Required]
    public Guid HotelId { get; set; }

    [Required]
    [Range(1, 5)]
    public int RatingValue { get; set; }

    public string Comment { get; set; }
    public virtual ApplicationUser User { get; set; }
}


public class RatingCreateDto
{
    public required string UserId { get; set; }
    [Required]
    public Guid HotelId { get; set; }
    [Required]
    [Range(1, 5)]
    public int RatingValue { get; set; }

    public string Comment { get; set; }
    public string Title { get; set; }
}


public class RatingUpdateDto
{
    public Guid Id { get; set; }
    [Required]
    [StringLength(450)]
    public string UserId { get; set; }

    [Required]
    public Guid HotelId { get; set; }

    [Required]
    [Range(1, 5)]
    public int RatingValue { get; set; }

    public string Comment { get; set; }
}

