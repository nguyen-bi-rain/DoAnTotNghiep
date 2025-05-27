namespace AuthJWT.Services.Interfaces
{
    public interface IS3Service
    {
        Task<Guid> UploadFileAsync(IFormFile file);
        Task<string> GetFileUrlAsync(string fileName);
        Task DeleteFileAsync(string fileName);
    }
}
