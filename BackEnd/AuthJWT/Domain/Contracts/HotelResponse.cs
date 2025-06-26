namespace AuthJWT.Domain.Contracts;

public class HotelResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string  ShortDescription { get; set; }
    public string? Thumnail { get; set; }
    public string RatingValue { get; set; }
    public int RatingCount { get; set; }    
    public string Location { get; set; }
    public decimal Price { get; set; }
}


