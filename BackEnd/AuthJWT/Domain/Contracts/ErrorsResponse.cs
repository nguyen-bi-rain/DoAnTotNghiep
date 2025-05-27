namespace AuthJWT.Domain.Contracts
{
    public class ErrorsResponse
    {
        public string Title { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }

    }
}
