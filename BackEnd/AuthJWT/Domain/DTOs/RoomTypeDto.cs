using System.ComponentModel.DataAnnotations;

namespace AuthJWT.Domain.DTOs
{
    public class RoomTypeDto
    {
        public Guid Id { get; set; }
        public string RoomTypeName { get; set; }
        public string ShortDescription { get; set; }
    }
    public class RoomTypeCreateDto
    {
        [Required]
        public string RoomTypeName { get; set; }
        [Required]
        [MaxLength(255)]
        public string ShortDescription { get; set; }
    }
    public class RoomTypeUpdateDto
    {
        [Required]
        public string RoomTypeName { get; set; }
        [Required]
        [MaxLength(255)]
        public string ShortDescription { get; set; }
    }
}