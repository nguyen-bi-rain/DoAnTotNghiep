using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Exceptions;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Services.Implements
{
    public class RoomService : IRoomService
    {
        private readonly IRoomImageService _roomImageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomConveniceService _roomConveniceService;
        private readonly IS3Service _s3Service;
        private readonly IMapper _mapper;

        public RoomService(IS3Service s3Service, IUnitOfWork unitOfWork, IMapper mapper, IRoomImageService roomImageService, IRoomConveniceService roomConveniceService)
        {
            _s3Service = s3Service;
            _roomImageService = roomImageService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _roomConveniceService = roomConveniceService;
        }

        public async Task<RoomCreateDto> CreateRoomAsync(RoomCreateDto room, List<IFormFile> files)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var model = _mapper.Map<Room>(room);
                model.AvailableRooms = room.NumberOfRooms;
                var entity = await _unitOfWork.RoomRepository.AddAsync(model);

                if (room.Conveniences != null && room.Conveniences.Any())
                {
                    foreach (var convenience in room.Conveniences)
                    {
                        var roomConvenice = new RoomConvenienceCreateDto
                        {
                            RoomId = entity.Id,
                            ConvenienceId = convenience.ConvenienceId
                        };

                        await _roomConveniceService.AddRoomConveniceAsync(roomConvenice);
                    }
                }
                if (files != null && files.Any())
                {
                    await _roomImageService.AddRoomImageAsync(entity.Id, files);
                }
                var res = await _unitOfWork.SaveChangesAsync();
                if (res <= 0)
                {
                    throw new DatabaseBadRequestException("Failed to create room.");
                }
                await transaction.CommitAsync();
                return room;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteRoomAsync(Guid id)
        {
            var room = await _unitOfWork.RoomRepository.GetQuery()
                                                        .Include(x => x.RoomImages)
                                                        .FirstOrDefaultAsync(q => q.Id == id);
            if (room == null)
            {
                throw new ResourceNotFoundException($"Room with Id : {id} not found");
            }
            if (room.RoomImages != null && room.RoomImages.Any())
            {
                foreach (var image in room.RoomImages)
                {
                    await _roomImageService.DeleteRoomImageAsync(image.Id);
                }
            }
            _unitOfWork.RoomRepository.Delete(room);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
        {
            var rooms = await _unitOfWork.RoomRepository.GetQuery()
                                                        .Include(x => x.RoomImages)
                                                        .Include(x => x.Conveniences)
                                                        .ToListAsync();
            if (rooms == null || !rooms.Any())
            {
                throw new KeyNotFoundException("No rooms found.");
            }
            return _mapper.Map<IEnumerable<RoomDto>>(rooms);
        }

        public async Task<IEnumerable<RoomDto>> GetAllRoomsByHotelIdAsync(Guid hotelId)
        {
            var rooms = _unitOfWork.RoomRepository.GetQuery(r => r.HotelId == hotelId)
                                                    .Include(r => r.RoomImages)
                                                    .Include(r => r.Conveniences)
                                                    .AsQueryable();

            if (rooms == null || !await rooms.AnyAsync())
            {
                throw new ResourceNotFoundException("No rooms found for this hotel.");
            }
            var models = await rooms.ToListAsync();
            var roomDtos = _mapper.Map<IEnumerable<RoomDto>>(models);

            foreach (var roomDto in roomDtos)
            {
                if (roomDto.RoomImages != null && roomDto.RoomImages.Any())
                {
                    foreach (var image in roomDto.RoomImages)
                    {
                        image.ImageUrl = await _s3Service.GetFileUrlAsync(image.ImageUrl.ToString());
                    }
                }
            }
            return roomDtos;
        }

        public async Task<RoomDto> GetRoomByIdAsync(Guid id)
        {
            var room = await _unitOfWork.RoomRepository.GetByIdAsync(id);
            return room == null ? throw new ResourceNotFoundException($"Room with Id : {id} not found") : _mapper.Map<RoomDto>(room);
        }

        public async Task<PaginateList<RoomDto>> GetRoomsByHotelIdAsync(Guid hotelId, int pageIndex = 1, int pageSize = 10, string? search = null)
        {
            var rooms = _unitOfWork.RoomRepository.GetQuery(r => r.HotelId == hotelId)
                                                    .Include(r => r.RoomImages)
                                                    .Include(r => r.Conveniences)
                                                    .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                rooms = rooms.Where(r => r.RoomName.Contains(search) || r.Description.Contains(search));
            }

            if (rooms == null || !await rooms.AnyAsync())
            {
                throw new ResourceNotFoundException("No rooms found for this hotel.");
            }
            var models = await rooms.ToListAsync();
            var roomDtos = _mapper.Map<IEnumerable<RoomDto>>(models);

            foreach (var roomDto in roomDtos)
            {
                if (roomDto.RoomImages != null && roomDto.RoomImages.Any())
                {
                    foreach (var image in roomDto.RoomImages)
                    {
                        image.ImageUrl = await _s3Service.GetFileUrlAsync(image.ImageUrl.ToString());
                    }
                }
            }
            var paging = PaginateList<RoomDto>.Create(roomDtos, pageIndex, pageSize);
            return paging;
        }

        public async Task UpdateQuantityOfRoomAsync(Guid id, int quantity)
        {
            var room = await _unitOfWork.RoomRepository.GetByIdAsync(id);
            if (room == null)
            {
                throw new ResourceNotFoundException($"Room with Id : {id} not found");
            }
            room.AvailableRooms += quantity;
            _unitOfWork.RoomRepository.Update(room);
        }

        public async Task<RoomUpdateDto> UpdateRoomAsync(RoomUpdateDto room, List<IFormFile>? files)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var existingRoom = await _unitOfWork.RoomRepository.GetQuery(r => r.Id == room.Id)
                        .Include(r => r.Conveniences)
                        .Include(r => r.RoomImages)
                        .FirstOrDefaultAsync();

                    if (existingRoom == null)
                    {
                        throw new ResourceNotFoundException($"Room with Id : {room.Id} not found");
                    }

                    // Update main fields
                    _mapper.Map(room, existingRoom);

                    await UpdateRoomConveniencesAsync(existingRoom, room.Conveniences);
                    await UpdateRoomImagesAsync(existingRoom, files);

                    _unitOfWork.RoomRepository.Update(existingRoom);
                    var res = await _unitOfWork.SaveChangesAsync();
                    if (res <= 0)
                    {
                        throw new DatabaseBadRequestException("Failed to update room.");
                    }

                    await transaction.CommitAsync();

                    return _mapper.Map<RoomUpdateDto>(existingRoom);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        private async Task UpdateRoomConveniencesAsync(Room existingRoom, List<RoomConvenienceDto>? conveniences)
        {
            if (conveniences != null && conveniences.Count > 0)
            {
                // Remove old conveniences
                var oldConveniences = existingRoom.Conveniences.ToList();
                foreach (var conv in oldConveniences)
                {
                    _unitOfWork.RoomConvenienceRepository.Delete(conv);
                }
                // Add new conveniences
                foreach (var convDto in conveniences)
                {
                    var conv = _mapper.Map<RoomConvenience>(convDto);
                    conv.RoomId = existingRoom.Id;
                    await _unitOfWork.RoomConvenienceRepository.AddAsync(conv);
                }
            }
        }

        private async Task UpdateRoomImagesAsync(Room existingRoom, List<IFormFile>? files)
        {
            if (files != null && files.Any())
            {
                // Remove old images
                if (existingRoom.RoomImages != null && existingRoom.RoomImages.Any())
                {
                    foreach (var image in existingRoom.RoomImages.ToList())
                    {
                        await _roomImageService.DeleteRoomImageAsync(image.Id);
                    }
                }
                // Add new images
                await _roomImageService.AddRoomImageAsync(existingRoom.Id, files);
            }
        }
    }
}