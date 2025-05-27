using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Services.Implements
{
    public class PolicyService : IPolicyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PolicyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task AddPolicyAsync(IEnumerable<PolicyDto> policyDtos)
        {
            var policies = _mapper.Map<IEnumerable<PolicyDto>, IEnumerable<Policy>>(policyDtos);
            foreach (var policy in policies)
            {
                await _unitOfWork.PolicyRepository.AddAsync(policy);
            }
            var res = await _unitOfWork.SaveChangesAsync();
            if (res <= 0)
            {
                throw new Exception("Failed to create policy.");
            }
        }

        public async Task<IEnumerable<PolicyDto>> GetPoliciesByHotelIdAsync(Guid hotelId)
        {
            var policies = await _unitOfWork.PolicyRepository.GetQuery()
                .Where(x => x.HotelId == hotelId)
                .ToListAsync();
            if (policies == null || !policies.Any())
            {
                throw new Exception($"No policies found for hotel with Id : {hotelId}");
            }
            return _mapper.Map<IEnumerable<Policy>, IEnumerable<PolicyDto>>(policies);
        }
    }
}