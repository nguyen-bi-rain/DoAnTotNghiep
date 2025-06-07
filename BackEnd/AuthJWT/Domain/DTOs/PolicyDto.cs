namespace AuthJWT.Domain.DTOs
{
    public class PolicyDto
    {
        public Guid Id { get; set; }
        public string PolicyType { get; set; }  
        public string PolicyDetails { get; set; }
    }
}