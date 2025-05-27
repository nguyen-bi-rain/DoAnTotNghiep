namespace AuthJWT.Domain.Entities.Common
{
    public class HotelImage : BaseEntity
    {
        public Guid? HotelId { get; set; }
        public Guid ImageUrl { get; set; }
        public string ImageType { get; set; } 
        public Hotel Hotel { get; set; }
    }
}