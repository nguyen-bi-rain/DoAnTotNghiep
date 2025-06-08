using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.DTOs.HotelDto;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Exceptions;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Services.Implements
{
    public class HotelService : IHotelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHotelImageService _hotelImageService;
        private readonly IRoomService _roomService;
        private readonly IS3Service _s3Service;

        public HotelService(IRoomService roomService, IUnitOfWork unitOfWork, IMapper mapper, IHotelImageService hotelImageService, IS3Service s3Service)
        {
            _roomService = roomService;
            _s3Service = s3Service;
            _hotelImageService = hotelImageService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateHotelAsync(HotelCreateDto hotel, List<IFormFile>? files)
        {
            var existingHotel = await _unitOfWork.HotelRepository.GetQuery(h => h.Name == hotel.Name || h.Address == hotel.Address).FirstOrDefaultAsync();
            if (existingHotel != null)
            {
                throw new ResourceUniqueException("Hotel with the same name already exists.");
            }

            // Map hotel but exclude Policies and Conveniences collections
            var model = _mapper.Map<Hotel>(hotel);
            // Clear any collections that might have been mapped
            model.Policies = new List<Policy>();
            model.HotelConveniences = new List<HotelConvenience>();

            var res = await _unitOfWork.HotelRepository.AddAsync(model);

            // Handle hotel images
            if (files != null && files.Count > 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    var hotelImage = new HotelImageCreateDto
                    {
                        HotelId = res.Id,
                        ImageType = i == 0 ? "Thumbnail" : "Hotel"
                    };
                    await _hotelImageService.CreateHotelImageAsync(hotelImage, files[i]);
                }
            }

            // Add policies
            if (hotel.Policies != null)
            {
                foreach (var item in hotel.Policies)
                {
                    var policy = new Policy
                    {
                        HotelId = res.Id,
                        CreatedAt = DateTime.UtcNow,
                        PolicyDetails = item.PolicyDetails,
                        PolicyType = item.PolicyType
                    };
                    await _unitOfWork.PolicyRepository.AddAsync(policy);
                }
            }

            // Add conveniences
            if (hotel.Conveniences != null)
            {
                foreach (var item in hotel.Conveniences)
                {
                    var convenience = _mapper.Map<HotelConvenience>(item);
                    convenience.HotelId = res.Id;
                    await _unitOfWork.HotelConvenienceRepository.AddAsync(convenience);
                }
            }

            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                throw new DatabaseBadRequestException("Failed to create hotel.");
            }
        }

        public async Task<bool> DeleteHotelAsync(Guid id)
        {
            var hotel = await _unitOfWork.HotelRepository.GetByIdAsync(id);
            if (hotel == null)
            {
                throw new ResourceNotFoundException($"Hotel with ID {id} not found.");
            }
            _unitOfWork.HotelRepository.Delete(hotel);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<HotelDto>> GetAllHotelsAsync()
        {
            var hotels = await _unitOfWork.HotelRepository.GetQuery().Include(x => x.Ratings).ToListAsync();
            return _mapper.Map<IEnumerable<HotelDto>>(hotels);
        }

        public async Task<IEnumerable<HotelResponse>> GetAllHotelsResponseAsync()
        {
            var hotels = await _unitOfWork.HotelRepository.GetQuery()
                .Include(f => f.Rooms)
                .Include(h => h.Ratings)
                .Include(h => h.HotelImages).ToListAsync();

            return _mapper.Map<IEnumerable<HotelResponse>>(hotels);
        }

        public async Task<HotelDetailResponse> GetHotelByIdAsync(Guid id)
        {
            var hotel = await _unitOfWork.HotelRepository.GetQuery()
                .Include(x => x.HotelImages)
                .Include(x => x.Location)
                .Include(x => x.HotelConveniences)
                .ThenInclude(hc => hc.Convenience)
                .Include(x => x.Policies)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (hotel == null)
            {
                throw new ResourceNotFoundException($"Hotel with ID {id} not found.");
            }

            // Get conveniences as DTOs
            var conveniences = new List<ConvenienceDto>();
            foreach (var item in hotel.HotelConveniences)
            {
                var convenience = await _unitOfWork.ConvenienceRepository.GetByIdAsync(item.ConvenienceId);
                if (convenience != null)
                {
                    conveniences.Add(_mapper.Map<ConvenienceDto>(convenience));
                }
            }

            // Get hotel images as DTOs
            var hotelImages = await _hotelImageService.GetImageBytHoelId(id);

            // Get rooms as RoomHotelReponse DTOs

            // Get policies
            var policies = hotel.Policies != null ? hotel.Policies.ToList() : new List<Policy>();

            var hotelDetail = new HotelDetailResponse
            {
                Id = hotel.Id,
                Name = hotel.Name,
                LocationId = hotel.LocationId,
                Address = hotel.Address,
                Description = hotel.Description,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                IsApproved = hotel.IsApproved,
                Conveniences = conveniences.Select(c => new ConvenienceDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }).ToList(),
                HotelImages = hotelImages.ToList(),
                Policy = _mapper.Map<List<PolicyDto>>(policies)
            };

            return hotelDetail;
        }

        public string GetHotelIdByUserId(string Userid)
        {
            var hotelId = _unitOfWork.HotelRepository.GetQuery().FirstOrDefault(x => x.OwnerId == Userid);
            return hotelId != null ? hotelId.Id.ToString() : string.Empty;
        }

        public async Task<PaginateList<HotelResponse>> SearchAvailableRoom(DateTime fromDate, DateTime endDate, int numberGuest, string location, int pageNumber = 1, int pageSize = 10, int sort = 1)
        {
            // Query hotels with available rooms
            var query = _unitOfWork.HotelRepository.GetQuery()
                .Include(h => h.Location)
                .Include(h => h.HotelImages)
                .Include(h => h.Ratings)
                .Include(h => h.Rooms)
                    .ThenInclude(r => r.Bookings)
                        .ThenInclude(b => b.Booking)
                .Where(h => h.IsApproved && h.Location.City == location);

            // Filter hotels with at least one available room
            var hotels = await query.ToListAsync();

            var availableHotels = hotels
                .Select(hotel =>
                {
                    var availableRooms = hotel.Rooms
                        .Where(r => r.Capacity >= numberGuest &&
                            !r.Bookings.Any(br =>
                                (br.Booking.Status == "confirmed" || br.Booking.Status == "Pending") &&
                                fromDate < br.Booking.CheckOutDate &&
                                endDate > br.Booking.CheckInDate
                            ))
                        .ToList();

                    return new
                    {
                        Hotel = hotel,
                        Rooms = availableRooms
                    };
                })
                .Where(x => x.Rooms.Any())
                .ToList();

            // Project to HotelResponse
            var hotelResponses = new List<HotelResponse>();
            foreach (var h in availableHotels)
            {
                var thumbnail = h.Hotel.HotelImages
                    .Where(i => i.ImageType == "Thumbnail")
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault();

                var ratingValue = h.Hotel.Ratings.Any()
                    ? h.Hotel.Ratings.Average(r => r.RatingValue)
                    : 0;

                var hotelResponse = new HotelResponse
                {
                    Id = h.Hotel.Id,
                    Name = h.Hotel.Name,
                    ShortDescription = h.Hotel.ShortDescription,
                    Thumnail = !string.IsNullOrEmpty(thumbnail.ToString()) ? await _s3Service.GetFileUrlAsync(thumbnail.ToString()) : string.Empty,
                    RatingValue = ratingValue.ToString("0.0"),
                    Location = $"{h.Hotel.Location.City}, {h.Hotel.Location.Country}",
                    Price = h.Rooms.Min(r => r.PricePerNight),
                    RatingCount = h.Hotel.Ratings.Count,
                    
                };
                hotelResponses.Add(hotelResponse);
            }

            // Sorting
            switch (sort)
            {
                case 1:
                    hotelResponses = hotelResponses.OrderBy(h => h.Price).ToList();
                    break;
                case 2:
                    hotelResponses = hotelResponses.OrderByDescending(h => h.Price).ToList();
                    break;
                case 3:
                    hotelResponses = hotelResponses.OrderBy(h => double.TryParse(h.RatingValue, out var rv) ? rv : 0).ToList();
                    break;
                case 4:
                    hotelResponses = hotelResponses.OrderByDescending(h => double.TryParse(h.RatingValue, out var rv) ? rv : 0).ToList();
                    break;
            }

            // Pagination
            var pagedList = PaginateList<HotelResponse>.Create(hotelResponses, pageNumber, pageSize);
            return pagedList;
        }

        public async Task<HotelUpdateDto> UpdateHotelAsync(HotelUpdateDto hotel, List<IFormFile>? files)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var existingHotel = await _unitOfWork.HotelRepository.GetQuery()
                        .Include(h => h.Policies)
                        .Include(h => h.HotelConveniences)
                        .Include(h => h.HotelImages)
                        .FirstOrDefaultAsync(h => h.Id == hotel.Id);

                    if (existingHotel == null)
                    {
                        throw new ResourceNotFoundException($"Hotel with ID {hotel.Id} not found.");
                    }

                    // Check for unique name or address (if changed)
                    var duplicateHotel = await _unitOfWork.HotelRepository.GetQuery(
                        h => (h.Name == hotel.Name || h.Address == hotel.Address) && h.Id != hotel.Id
                    ).FirstOrDefaultAsync();
                    if (duplicateHotel != null)
                    {
                        throw new ResourceUniqueException("Another hotel with the same name or address already exists.");
                    }

                    // Update main fields
                    existingHotel.Name = hotel.Name;
                    existingHotel.LocationId = hotel.LocationId;
                    existingHotel.Address = hotel.Address;
                    existingHotel.Description = hotel.Description;
                    existingHotel.PhoneNumber = hotel.PhoneNumber;
                    existingHotel.Email = hotel.Email;

                    // Update Policies
                    if (hotel.Policies != null && hotel.Policies.Count > 0)
                    {
                        // Remove old policies
                        var oldPolicies = existingHotel.Policies.ToList();
                        foreach (var policy in oldPolicies)
                        {
                            _unitOfWork.PolicyRepository.Delete(policy);
                        }
                        // Add new policies
                        foreach (var policyDto in hotel.Policies)
                        {
                            var policy = _mapper.Map<Policy>(policyDto);
                            policy.HotelId = existingHotel.Id;
                            await _unitOfWork.PolicyRepository.AddAsync(policy);
                        }
                    }

                    // Update Conveniences
                    if (hotel.Conveniences != null && hotel.Conveniences.Count > 0)
                    {
                        // Remove old conveniences
                        var oldConveniences = existingHotel.HotelConveniences.ToList();
                        foreach (var conv in oldConveniences)
                        {
                            _unitOfWork.HotelConvenienceRepository.Delete(conv);
                        }
                        // Add new conveniences
                        foreach (var convDto in hotel.Conveniences)
                        {
                            var conv = _mapper.Map<HotelConvenience>(convDto);
                            conv.HotelId = existingHotel.Id;
                            await _unitOfWork.HotelConvenienceRepository.AddAsync(conv);
                        }
                    }

                    // Add new Hotel Images
                    if (files != null && files.Count > 0)
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            var hotelImage = new HotelImageCreateDto
                            {
                                HotelId = existingHotel.Id,
                                ImageType = i == 0 ? "Thumbnail" : "Hotel"
                            };
                            await _hotelImageService.CreateHotelImageAsync(hotelImage, files[i]);
                        }
                    }
                    _unitOfWork.HotelRepository.Update(existingHotel);
                    var result = await _unitOfWork.SaveChangesAsync();
                    if (result <= 0)
                    {
                        throw new DatabaseBadRequestException("Failed to update hotel.");
                    }

                    await transaction.CommitAsync();

                    return _mapper.Map<HotelUpdateDto>(existingHotel);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
