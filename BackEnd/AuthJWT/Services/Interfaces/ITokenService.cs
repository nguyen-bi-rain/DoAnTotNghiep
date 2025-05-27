using System.Security.Claims;
using AuthJWT.Domain.Entities.Security;

namespace AuthJWT.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
