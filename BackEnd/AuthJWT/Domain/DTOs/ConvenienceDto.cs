
using System.ComponentModel.DataAnnotations;
using AuthJWT.Domain.Enums;

namespace AuthJWT.Domain.DTOs;

public class ConvenienceDto
{
    public Guid Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    [Required]
    [StringLength(255)]
    public string Description { get; set; }
    public ConvenienceType Type { get; set; }
}

public class ConvenienceCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    [Required]
    [StringLength(255)]
    public string Description { get; set; }
    public ConvenienceType Type { get; set; }

}

public class ConvenienceUpdateDto
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    [Required]
    [StringLength(255)]
    public string Description { get; set; }
    public ConvenienceType Type { get; set; }

}
