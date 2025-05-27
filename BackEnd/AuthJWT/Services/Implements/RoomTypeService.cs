using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Exceptions;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Services.Implements
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<RoomTypeCreateDto> CreateRoomTypeAsync(RoomTypeCreateDto roomType)
        {
            var model = _mapper.Map<RoomType>(roomType);
            await _unitOfWork.RoomTypeRepository.AddAsync(model);
            var res = await _unitOfWork.SaveChangesAsync();
            if (res == 0)
            {
                throw new Exception("Error creating room type");
            }
            return roomType;
        }

        public async Task<bool> DeleteRoomTypeAsync(Guid id)
        {
            var roomtype = await _unitOfWork.RoomTypeRepository.GetByIdAsync(id);
            if (roomtype == null)
            {
                throw new ResourceNotFoundException("Room type not found");
            }
            _unitOfWork.RoomTypeRepository.Delete(roomtype);
            return await _unitOfWork.SaveChangesAsync() > 0;

        }

        public async Task<IEnumerable<RoomTypeDto>> GetAllRoomTypesAsync()
        {
            var roomTypes = await _unitOfWork.RoomTypeRepository.GetQuery().ToListAsync();
            if (roomTypes == null || !roomTypes.Any())
            {
                throw new ResourceNotFoundException("Room types not found");
            }
            var result = _mapper.Map<IEnumerable<RoomTypeDto>>(roomTypes);
            return result;
        }

        public async Task<RoomTypeDto> GetRoomTypeByIdAsync(Guid id)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.GetByIdAsync(id);
            if (roomType == null)
            {
                throw new ResourceNotFoundException("Room type not found");
            }
            return _mapper.Map<RoomTypeDto>(roomType);
        }

        public async Task<RoomTypeDto> UpdateRoomTypeAsync(Guid id, RoomTypeUpdateDto roomType)
        {
            var roomTypeToUpdate = await _unitOfWork.RoomTypeRepository.GetByIdAsync(id);
            if (roomTypeToUpdate == null)
            {
                throw new ResourceNotFoundException("Room type not found");
            }
            _mapper.Map(roomType, roomTypeToUpdate);
            _unitOfWork.RoomTypeRepository.Update(roomTypeToUpdate);
            var res = await _unitOfWork.SaveChangesAsync();
            if (res == 0)
            {
                throw new Exception("Error updating room type");
            }
            return _mapper.Map<RoomTypeDto>(roomTypeToUpdate);
        }
    }
}