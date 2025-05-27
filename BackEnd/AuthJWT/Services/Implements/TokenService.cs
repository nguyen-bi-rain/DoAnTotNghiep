using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.Entities.Security;
using AuthJWT.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AuthJWT.Services.Implements
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _secrectKey;
        private readonly string? _validIssuer;
        private readonly string? _validAudience;
        private readonly double _exprires;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TokenService> _logger;
        private readonly IHotelService _hotelService;
        public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager, ILogger<TokenService> logger,IHotelService hotelService)
        {
            _hotelService = hotelService;
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JWTSetting>();
            if (jwtSettings is null || string.IsNullOrEmpty(jwtSettings.Key))
            {
                throw new InvalidOperationException("JwtSettings is not configured");
            }
            _secrectKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            _validIssuer = jwtSettings.ValidIssuer;
            _validAudience = jwtSettings.ValidAudience;
            _exprires = Convert.ToDouble(jwtSettings.Expires);
            _userManager = userManager;
            _logger = logger;
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);
            return refreshToken;
        }
        private async Task<List<Claim>> GetClaims(ApplicationUser user)
        {

            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            if (user.isHotelOwner)
            {
                var hotelId = _hotelService.GetHotelIdByUserId(user.Id);
                if (hotelId == null)
                {
                    _logger.LogWarning("Hotel ID not found for user {UserId}", user.Id);
                    throw new InvalidOperationException("Hotel ID not found for user");
                }
                claims.Add(new Claim("hotelId", hotelId));
            }
            return claims;
        }
        private JwtSecurityToken GenerateOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            return new JwtSecurityToken(
                issuer: _validIssuer,
                audience: _validAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_exprires),
                signingCredentials: signingCredentials
            );
        }
        public async Task<string> GenerateToken(ApplicationUser user)
        {
            var signingCredentials = new SigningCredentials(_secrectKey, SecurityAlgorithms.HmacSha256);
            var claims = await GetClaims(user);
            var tokenOptions = GenerateOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _secrectKey,
                ValidateIssuer = true,
                ValidIssuer = _validIssuer,
                ValidateAudience = true,
                ValidAudience = _validAudience,
                ValidateLifetime = false 
            };
            try
            {
                return tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed.");
                throw;
            }
        }
    }
}
