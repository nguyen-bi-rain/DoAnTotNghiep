using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Exceptions;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Services.Implements
{
    public class HotelConvenienceService : IHotelConvenienceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HotelConvenienceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<HotelConvenienceDto> CreateHotelConvenienceAsync(HotelConvenienceDto hotelConvenience)
        {
            var model = _mapper.Map<HotelConvenience>(hotelConvenience);
            await _unitOfWork.HotelConvenienceRepository.AddAsync(model);
            var res = await _unitOfWork.SaveChangesAsync();
            if (res == 0)
            {
                throw new DatabaseBadRequestException("Error creating hotel convenience");
            }
            return hotelConvenience;
        }

        public async Task CreateHotelConveniencesAsync(Guid hotelId, IEnumerable<HotelConvenienceDto> hotelConveniences)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var hotelConvenienceEntities = _mapper.Map<IEnumerable<HotelConvenience>>(hotelConveniences);
                foreach (var hotelConvenience in hotelConvenienceEntities)
                {
                    hotelConvenience.HotelId = hotelId;
                    await _unitOfWork.HotelConvenienceRepository.AddAsync(hotelConvenience);
                }

                var res = await _unitOfWork.SaveChangesAsync();
                if (res == 0)
                {
                    throw new DatabaseBadRequestException("Error creating hotel conveniences");
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteHotelConvenienceAsync(Guid hotelId, Guid convenienceId)
        {
            var hotelConvenience = await _unitOfWork.HotelConvenienceRepository.GetByIdAsync(convenienceId);
            if (hotelConvenience == null)
            {
                throw new ResourceNotFoundException("Hotel convenience not found");
            }

            _unitOfWork.HotelConvenienceRepository.Delete(hotelConvenience);

            return await _unitOfWork.SaveChangesAsync() > 0;
        }


        public async Task<IEnumerable<HotelConvenienceDto>> GetAllHotelConveniencesAsync(Guid hotelId)
        {
            var hotelConveniences = await _unitOfWork.HotelConvenienceRepository.GetQuery(h => h.HotelId == hotelId).ToListAsync();
            return _mapper.Map<IEnumerable<HotelConvenienceDto>>(hotelConveniences);
        }

        public Task<HotelConvenienceDto> GetHotelConvenienceByIdAsync(Guid hotelId, Guid convenienceId)
        {
            throw new NotImplementedException();
        }

        public Task<HotelConvenienceDto> UpdateHotelConvenienceAsync(Guid hotelId, Guid convenienceId, HotelConvenienceDto hotelConvenience)
        {
            throw new NotImplementedException();
        }
    }

}