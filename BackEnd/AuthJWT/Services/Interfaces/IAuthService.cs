
using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.Entities.Security;

namespace AuthJWT.Services.Interfaces
{
    public interface IAuthService
    {
        public Task GoogleLoginAsync(string returnUrl);
        public Task<UserResponse> HandleGoogleCallbackAsync();
    }
}