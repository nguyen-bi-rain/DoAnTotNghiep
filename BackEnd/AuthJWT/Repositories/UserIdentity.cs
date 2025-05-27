using AuthJWT.Domain.Entities.Security;
using Microsoft.AspNetCore.Identity;

namespace AuthJWT.Repositories
{
    public class UserIdentity : IUserIdentity
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserIdentity(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual string UserId => GetCurrentUserAsync().Result?.Id ?? string.Empty;

        private async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            var email = _httpContextAccessor.HttpContext?.User?.Identity?.Name?.ToUpper();

            if (email == null)
            {
                return null;
            }

            var currentUser = await _userManager.FindByNameAsync(email);

            return currentUser;
        }

        public virtual string UserName => GetCurrentUserAsync().Result?.UserName ?? string.Empty;

    }
}