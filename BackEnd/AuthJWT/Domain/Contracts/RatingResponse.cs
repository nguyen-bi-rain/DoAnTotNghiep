namespace AuthJWT.Domain.Contracts
{
    public class RatingResponse
    {
        public Guid Id { get; set; }
        public int RatingValue { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        
    }
}