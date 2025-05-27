namespace AuthJWT.Domain.Contracts
{
    public class RatingResponse
    {
        public int RatingValue { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
        
    }
}