using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Exceptions;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Services.Implements
{
    public class RoomConvenienceService : IRoomConveniceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RoomConvenienceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddRoomConveniceAsync(RoomConvenienceCreateDto request)
        {
            var roomConvenience = _mapper.Map<RoomConvenience>(request);
            var room = await _unitOfWork.RoomRepository.GetByIdAsync(request.RoomId);
            if (room == null)
            {
                throw new ResourceNotFoundException("Room not found.");
            }
            var convenience = await _unitOfWork.ConvenienceRepository.GetByIdAsync(request.ConvenienceId);
            if (convenience == null)
            {
                throw new ResourceNotFoundException("Convenience not found.");
            }
            await _unitOfWork.RoomConvenienceRepository.AddAsync(roomConvenience);
            return true;
        }

        public async Task<List<Guid>> GetRoomConvenicesAsync(Guid roomId)
        {
            var roomConvenices = await _unitOfWork.RoomConvenienceRepository.GetQuery()
                .Where(rc => rc.RoomId == roomId)
                .Select(rc => rc.ConvenienceId)
                .ToListAsync();

            if (roomConvenices == null || !roomConvenices.Any())
            {
                throw new KeyNotFoundException("No room conveniences found.");
            }
            return roomConvenices;
        }

        public Task<bool> IsRoomConveniceExistAsync(Guid roomId, Guid convenienceId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveRoomConveniceAsync(Guid id)
        {
            var roomConvenice = await _unitOfWork.RoomConvenienceRepository.GetByIdAsync(id);
            if (roomConvenice == null)
            {
                throw new KeyNotFoundException("Room convenience not found.");
            }
            _unitOfWork.RoomConvenienceRepository.Delete(roomConvenice);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}