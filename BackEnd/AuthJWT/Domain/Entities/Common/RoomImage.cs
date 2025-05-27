namespace AuthJWT.Domain.Entities.Common
{
    public class RoomImage : BaseEntity
    {
        public Guid? RoomId { get; set; }
        public Guid ImageUrl { get; set; }
        public string ImageType { get; set; } 
        public Room Room { get; set; }
    }
}