using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuthJWT.Domain.Entities.Security;

namespace AuthJWT.Domain.Entities.Common;

public class Rating : BaseEntity
{
    [Required]
    [StringLength(450)]
    public string UserId { get; set; }
    [Required]
    public Guid HotelId { get; set; }
    [Required]
    [MaxLength(50)]
    public string Title { get; set; }
    [Required]
    [Range(1, 5)]
    public int RatingValue { get; set; }
    public string Comment { get; set; }
    [ForeignKey("HotelId")]
    public virtual Hotel Hotel { get; set; }
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }
}