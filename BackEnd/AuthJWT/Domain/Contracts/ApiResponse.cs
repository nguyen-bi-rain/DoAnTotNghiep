namespace AuthJWT.Domain.Contracts;

public class ApiResponse<T> where T : class
{
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public T? Data { get; set; }

    public static ApiResponse<T> Success(T data, string message = "", bool success = true, int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Message = message,
            IsSuccess = success,
            StatusCode = statusCode,
            Data = data
        };
    }
    public static ApiResponse<T> Failure(string message = "", int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Message = message,
            IsSuccess = false,
            StatusCode = statusCode,
            Data = null
        };
    }
}