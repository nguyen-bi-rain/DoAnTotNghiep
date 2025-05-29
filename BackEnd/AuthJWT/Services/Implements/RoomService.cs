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
            var roomDtos = _mapper.Map<List<RoomDto>>(rooms.ToList());

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
            var room = await _unitOfWork.RoomRepository.GetQuery()
                                                        .Include(x => x.RoomImages)
                                                        .Include(x => x.Conveniences)
                                                        .ThenInclude(rc => rc.Convenience)
                                                        .FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
            {
                throw new ResourceNotFoundException($"Room with Id : {id} not found");
            }
            var roomDto = _mapper.Map<RoomDto>(room);

            if (roomDto.RoomImages != null && roomDto.RoomImages.Any())
            {
                foreach (var image in roomDto.RoomImages)
                {
                    image.ImageUrl = await _s3Service.GetFileUrlAsync(image.ImageUrl.ToString());
                }
            }

            // Map conveniences with details from Convenience entity
            if (roomDto.Conveniences != null && roomDto.Conveniences.Any())
            {
                foreach (var conv in roomDto.Conveniences)
                {
                    // Find the matching RoomConvenience in the model to get full Convenience info
                    var roomConv = room.Conveniences.FirstOrDefault(c => c.ConvenienceId == conv.Id);
                    if (roomConv != null && roomConv.Convenience != null)
                    {
                        conv.Name = roomConv.Convenience.Name;
                        conv.Description = roomConv.Convenience.Description;
                        conv.Type = roomConv.Convenience.Type;
                    }
                }
            }

            return roomDto;
        }

        public async Task<PaginateList<RoomDto>> GetRoomsByHotelIdAsync(Guid hotelId,int capacity, int pageIndex = 1, int pageSize = 10, string? search = null,DateTime? checkIn = null, DateTime? checkOut = null)
        {
            var roomsQuery = _unitOfWork.RoomRepository.GetQuery(r => r.HotelId == hotelId)
                                                        .Include(r => r.RoomImages)
                                                        .Include(r => r.Conveniences)
                                                            .ThenInclude(rc => rc.Convenience)
                                                        .Include(r => r.Bookings)
                                                        .ThenInclude(b => b.Booking)
                                                        .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                roomsQuery = roomsQuery.Where(r => r.RoomName.Contains(search) || r.Description.Contains(search));
            }

            // Filter by capacity
            if (capacity > 0)
            {
                roomsQuery = roomsQuery.Where(r => r.Capacity >= capacity);
            }

            // Filter by availability in the given date range
            if (checkIn.HasValue && checkOut.HasValue)
            {
                var ci = checkIn.Value.Date;
                var co = checkOut.Value.Date;

                roomsQuery = roomsQuery.Where(room =>
                    // Room must have at least 1 available room
                    room.AvailableRooms > 0 &&
                    // Room is available if all bookings do not overlap with requested range
                    !room.Bookings.Any(b =>
                        // Only consider bookings that are not cancelled
                        b.Booking.Status != "Cancelled" &&
                        (
                            (b.Booking.CheckInDate < co && b.Booking.CheckOutDate > ci)
                        )
                    )
                );
            }

            var totalCount = await roomsQuery.CountAsync();
            if (totalCount == 0)
            {
                throw new ResourceNotFoundException("No rooms found for this hotel.");
            }

            var models = await roomsQuery
                .OrderBy(r => r.RoomName)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var roomDtos = _mapper.Map<List<RoomDto>>(models);

            foreach (var roomDto in roomDtos)
            {
                if (roomDto.RoomImages != null && roomDto.RoomImages.Any())
                {
                    foreach (var image in roomDto.RoomImages)
                    {
                        image.ImageUrl = await _s3Service.GetFileUrlAsync(image.ImageUrl.ToString());
                    }
                }

                // Map conveniences with details from Convenience entity
                if (roomDto.Conveniences != null && roomDto.Conveniences.Any())
                {
                    foreach (var conv in roomDto.Conveniences)
                    {
                        // Find the matching RoomConvenience in the model to get full Convenience info
                        var roomModel = models.FirstOrDefault(r => r.Id == roomDto.Id);
                        var roomConv = roomModel?.Conveniences.FirstOrDefault(c => c.ConvenienceId == conv.Id);
                        if (roomConv != null && roomConv.Convenience != null)
                        {
                            conv.Name = roomConv.Convenience.Name;
                            conv.Description = roomConv.Convenience.Description;
                            conv.Type = roomConv.Convenience.Type;
                        }
                    }
                }
            }

            var paging = new PaginateList<RoomDto>(roomDtos, totalCount, pageIndex, pageSize);
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
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            var existingRoom = await _unitOfWork.RoomRepository.GetQuery()
                                    .Include(x => x.Conveniences)
                                    .Include(x => x.RoomImages)
                                    .FirstOrDefaultAsync(r => r.Id == room.Id);
            if (existingRoom == null)
            {
                throw new ResourceNotFoundException($"Room with Id : {room.Id} not found");
            }

            existingRoom.RoomName = room.RoomName;
            existingRoom.HotelId = room.HotelId;
            existingRoom.RoomTypeId = room.RoomTypeId;
            existingRoom.Status = room.Status;
            existingRoom.ViewType = room.ViewType;
            existingRoom.PricePerNight = room.PricePerNight;
            existingRoom.Description = room.Description;
            existingRoom.Capacity = room.Capacity;
            existingRoom.NumberOfBeds = room.NumberOfBeds;
            existingRoom.BedType = room.BedType;

            if (room.NumberOfRooms > existingRoom.NumberOfRooms)
            {
                existingRoom.AvailableRooms += (room.NumberOfRooms - existingRoom.NumberOfRooms);
            }
            else if (room.NumberOfRooms < existingRoom.NumberOfRooms)
            {
                var diff = existingRoom.NumberOfRooms - room.NumberOfRooms;
                existingRoom.AvailableRooms = Math.Max(0, existingRoom.AvailableRooms - diff);
            }
            existingRoom.NumberOfRooms = room.NumberOfRooms;

            // Handle conveniences (add new and remove old)
            var existingConvenienceIds = existingRoom.Conveniences.Select(c => c.ConvenienceId).ToList();
            var newConvenienceIds = room.Conveniences?.Select(conv => conv.ConvenienceId).ToList() ?? new List<Guid>();

            // Add new conveniences
            var conveniencesToAdd = newConvenienceIds.Except(existingConvenienceIds).ToList();
            foreach (var convenienceId in conveniencesToAdd)
            {
                var newConvenience = new RoomConvenienceCreateDto
                {
                    RoomId = existingRoom.Id,
                    ConvenienceId = convenienceId
                };
                await _roomConveniceService.AddRoomConveniceAsync(newConvenience);
            }

            // Remove old conveniences
            var conveniencesToRemove = existingConvenienceIds.Except(newConvenienceIds).ToList();
            foreach (var convenienceId in conveniencesToRemove)
            {
                var roomConvenience = existingRoom.Conveniences.FirstOrDefault(c => c.ConvenienceId == convenienceId);
                if (roomConvenience != null)
                {
                    _unitOfWork.RoomConvenienceRepository.Delete(roomConvenience);
                }
            }

            // Only add new images if provided
            if (files != null && files.Any())
            {
                await _roomImageService.AddRoomImageAsync(existingRoom.Id, files);
            }

            _unitOfWork.RoomRepository.Update(existingRoom);
            var res = await _unitOfWork.SaveChangesAsync();
            if (res <= 0)
            {
                await transaction.RollbackAsync();
                throw new DatabaseBadRequestException("Failed to update room.");
            }
            await transaction.CommitAsync();
            return room;
        }

    }
}