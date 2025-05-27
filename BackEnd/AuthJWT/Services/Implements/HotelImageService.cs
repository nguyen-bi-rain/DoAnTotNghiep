using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Exceptions;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Services.Implements
{
    public class HotelImageService : IHotelImageService
    {
        private readonly IS3Service _s3Service;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HotelImageService(IS3Service s3Service, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _s3Service = s3Service;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateHotelImageAsync(HotelImageCreateDto hotelImageDto, IFormFile file)
        {
            var hotelImage = _mapper.Map<HotelImage>(hotelImageDto);
            if (file != null)
            {
                var fileName = await _s3Service.UploadFileAsync(file);
                hotelImage.ImageUrl = fileName;
            }
            await _unitOfWork.HotelImageRepository.AddAsync(hotelImage);
        }


        public async Task DeleteHotelImageAsync(Guid id)
        {
            var hotelImage = await _unitOfWork.HotelImageRepository.GetByIdAsync(id);
            if (hotelImage == null)
            {
                throw new ResourceNotFoundException("Hotel image not found");
            }
            await _s3Service.DeleteFileAsync(hotelImage.ImageUrl.ToString());
            _unitOfWork.HotelImageRepository.Delete(hotelImage);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<HotelImageDto>> GetImageBytHoelId(Guid hotelId)
        {
            var hotelImage = await _unitOfWork.HotelImageRepository.GetQuery(x => x.HotelId == hotelId).ToListAsync();
            var model = _mapper.Map<IEnumerable<HotelImageDto>>(hotelImage);
            foreach (var image in model)
            {
                image.ImageUrl = await _s3Service.GetFileUrlAsync(image.ImageUrl.ToString());
            }

            return model;
        }

        public async Task UpdatHotelImageAsync(Guid id, IFormFile file)
        {
            var hotelImage = await _unitOfWork.HotelImageRepository.GetByIdAsync(id);
            if (hotelImage == null)
            {
                throw new ResourceNotFoundException("Hotel image not found");
            }
            if (file != null)
            {
                if (!string.IsNullOrEmpty(hotelImage.ImageUrl.ToString()))
                {
                    await _s3Service.DeleteFileAsync(hotelImage.ImageUrl.ToString());
                }
                var fileName = await _s3Service.UploadFileAsync(file);
                hotelImage.ImageUrl = fileName;
            }
            _unitOfWork.HotelImageRepository.Update(hotelImage);
            
        }
    }
}