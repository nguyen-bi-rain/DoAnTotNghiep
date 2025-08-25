using System.Security.Claims;
using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.Entities.Security;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;

namespace AuthJWT.Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task GoogleLoginAsync(string returnUrl)
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = returnUrl,
                Items =
                {
                    { "scheme", GoogleDefaults.AuthenticationScheme }
                }
            };

            await _httpContextAccessor.HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, authenticationProperties);
        }

        public async Task<UserResponse> HandleGoogleCallbackAsync()
        {
            var result = await _httpContextAccessor.HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return null;

            var googleUser = result.Principal;
            var email = googleUser.FindFirstValue(ClaimTypes.Email);
            var name = googleUser.FindFirstValue(ClaimTypes.Name);
            var picture = googleUser.FindFirstValue("picture");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = name,
                    LastName = "",
                    Avatar = picture
                };

                var resultCreate = await _userManager.CreateAsync(user);
                if (!resultCreate.Succeeded)
                {
                    throw new Exception("Failed to create user from Google login.");
                }
            }
            var token = await _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            return new UserResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
            };
        }
    }
}