using System.ComponentModel.DataAnnotations;

namespace AuthJWT.Domain.DTOs
{
    public class RoomDto
    {
        public Guid Id { get; set; }
        public string RoomName { get; set; }
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
        public string Status { get; set; }
        public string ViewType { get; set; }
        public decimal PricePerNight { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public int NumberOfBeds { get; set; }
        public int NumberOfRooms { get; set; }
        public int AvailableRooms { get; set; }
        public string BedType { get; set; }
        public virtual ICollection<ConvenienceDto> Conveniences { get; set; } = new List<ConvenienceDto>();
        public virtual ICollection<RoomImageDto> RoomImages { get; set; } = new List<RoomImageDto>();
    }
    public class RoomResponse
    {
        public Guid Id { get; set; }
        public string RoomName { get; set; }
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
        public string Status { get; set; }
        public string ViewType { get; set; }
        public decimal PricePerNight { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public int NumberOfBeds { get; set; }
        public int NumberOfRooms { get; set; }
        public int AvailableRooms { get; set; }
        public string BedType { get; set; }
    }

    public class RoomHotelReponse
    {
        public Guid Id { get; set; }
        public string RoomName { get; set; }
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
        public string Status { get; set; }
        public string ViewType { get; set; }
        public decimal PricePerNight { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public int NumberOfBeds { get; set; }
        public int NumberOfRooms { get; set; }
        public int AvailableRooms { get; set; }
        public string BedType { get; set; }
        public List<ConvenienceDto> Conveniences { get; set; } = new List<ConvenienceDto>();
    }

    public class RoomCreateDto
    {
        [Required]
        public string RoomName { get; set; }
        [Required]
        public Guid HotelId { get; set; }
        [Required]
        public Guid RoomTypeId { get; set; }
        [Required]
        public string Status { get; set; } = "Available";
        [Required]
        public string ViewType { get; set; }
        [Required]
        public decimal PricePerNight { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public int NumberOfBeds { get; set; }
        [Required]
        public int NumberOfRooms { get; set; }
        [Required]
        public string BedType { get; set; }
        public ICollection<RoomConvenienceCreateDto> Conveniences { get; set; }
    }

    public class RoomUpdateDto
    {
        public Guid Id { get; set; }
        [Required]
        public string RoomName { get; set; }
        [Required]
        public Guid HotelId { get; set; }
        [Required]
        public Guid RoomTypeId { get; set; }
        [Required]
        public string Status { get; set; } = "Available";
        [Required]
        public string ViewType { get; set; }
        [Required]
        public decimal PricePerNight { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public int NumberOfBeds { get; set; }
        [Required]
        public int NumberOfRooms { get; set; }
        [Required]
        public string BedType { get; set; }
        public ICollection<RoomConvenienceUpdateDto> Conveniences { get; set; }
    }
}