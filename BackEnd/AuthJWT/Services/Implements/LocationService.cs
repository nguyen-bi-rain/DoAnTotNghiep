using AuthJWT.Domain.DTOs;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Services.Implements
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public LocationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        {
            var locations = await _unitOfWork.LocationRepository.GetQuery().OrderBy(x => x.City).ToListAsync();
            if (locations == null || !locations.Any())
            {
                return Enumerable.Empty<LocationDto>();
            }

            var locationDtos = _mapper.Map<IEnumerable<LocationDto>>(locations);
            return locationDtos;
        }
    }
}