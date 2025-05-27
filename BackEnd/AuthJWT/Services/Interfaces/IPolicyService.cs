using AuthJWT.Domain.DTOs;

namespace AuthJWT.Services.Interfaces
{
    public interface IPolicyService
    {
        Task AddPolicyAsync(IEnumerable<PolicyDto> policyDtos);
        Task<IEnumerable<PolicyDto>> GetPoliciesByHotelIdAsync(Guid hotelId);

    }
}