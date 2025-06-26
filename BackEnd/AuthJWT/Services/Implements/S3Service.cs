using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using AuthJWT.Domain.DTOs;
using AuthJWT.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace AuthJWT.Services.Implements
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Service(IOptions<AWSSetting> options)
        {
            var config = options.Value;
            _bucketName = config.BucketName;

            _s3Client = new AmazonS3Client(
                config.AccessKey,
                config.SecretKey,
                Amazon.RegionEndpoint.GetBySystemName(config.Region)
            );
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = $"BookingApp/{fileName}"
            };

            await _s3Client.DeleteObjectAsync(request);
        }

        public async Task<string> GetFileUrlAsync(string fileName)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = $"BookingApp/{fileName}",
                Expires = DateTime.UtcNow.AddMinutes(15)
            };

            return await _s3Client.GetPreSignedURLAsync(request);
        }

        public async Task<Guid> UploadFileAsync(IFormFile file)
        {
            try
            {
                const long fileSizeLimit = 5 * 1024 * 1024;

                if (file.Length > fileSizeLimit)
                {
                    throw new InvalidOperationException("File size exceeds the limit of 5 MB.");
                }

                var fileTransferUtility = new TransferUtility(_s3Client);
                var key = Guid.NewGuid();
                var fileName = $"BookingApp/{key}";

                using var stream = file.OpenReadStream();
                var request = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    Key = fileName,
                    BucketName = _bucketName,
                    ContentType = file.ContentType,
                    Metadata = {
                    ["file-name"] = file.FileName
                }
                };

                await fileTransferUtility.UploadAsync(request);
                return key;
            }
            catch (AmazonS3Exception ex)
            {
                throw new InvalidOperationException($"Error uploading file to S3: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while uploading the file: {ex.Message}", ex);
            }
        }
    }
}
