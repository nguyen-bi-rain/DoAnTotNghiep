using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using AutoMapper;

namespace AuthJWT.Services.Implements
{
    public class RoomImageService : IRoomImageService
    {
        private readonly IS3Service _s3Service;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomImageService(IS3Service s3Service, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _s3Service = s3Service;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task AddRoomImageAsync(Guid roomId, List<IFormFile> images)
        {
            var roomImages = new List<RoomImageCreateDto>();
            foreach (var image in images)
            {
                var imageUrl = await _s3Service.UploadFileAsync(image);
                roomImages.Add(new RoomImageCreateDto
                {
                    RoomId = roomId,
                    ImageUrl = imageUrl,
                    ImageType = image.ContentType,
                });
            }
            var roomImageEntities = _mapper.Map<IEnumerable<RoomImage>>(roomImages);
            foreach (var roomImage in roomImageEntities)
            {
                roomImage.RoomId = roomId;
                await _unitOfWork.RoomImageRepository.AddAsync(roomImage);
            }
        }

        public async Task<bool> DeleteRoomImageAsync(Guid imageId)
        {
            try
            {
                var roomImage = await _unitOfWork.RoomImageRepository.GetByIdAsync(imageId);
                if (roomImage == null)
                {
                    throw new KeyNotFoundException("Room image not found.");
                }
                await _s3Service.DeleteFileAsync(roomImage.ImageUrl.ToString());

                _unitOfWork.RoomImageRepository.Delete(roomImage);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                throw new Exception("An error occurred while deleting the room image.", ex);
            }
        }
    }
}