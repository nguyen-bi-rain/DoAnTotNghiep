using AuthJWT.Domain.Entities.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

public class HotelOwnerTokenProvider : IUserTwoFactorTokenProvider<ApplicationUser>
{
    private readonly DataProtectionTokenProviderOptions _options;

    public HotelOwnerTokenProvider(IOptions<DataProtectionTokenProviderOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> GenerateAsync(string purpose, UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        var protector = CreateProtector(purpose, manager, user);

        // Time-limited token that expires after the configured time
        var ms = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var data = $"{user.Id}|{user.Email}|{ms}";
        return protector.Protect(data);
    }

    public async Task<bool> ValidateAsync(string purpose, string token, UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        var protector = CreateProtector(purpose, manager, user);

        try
        {
            var data = protector.Unprotect(token);
            var parts = data.Split('|');

            if (parts.Length != 3)
                return false;

            var userId = parts[0];
            var email = parts[1];
            var ms = long.Parse(parts[2]);

            if (userId != user.Id || email != user.Email)
                return false;

            var creationTime = DateTimeOffset.FromUnixTimeMilliseconds(ms);
            var expirationTime = creationTime + _options.TokenLifespan;

            return expirationTime >= DateTimeOffset.UtcNow;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        return !user.isHotelOwner; // Only generate tokens for users who aren't already hotel owners
    }

    private IDataProtector CreateProtector(string purpose, UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        return DataProtectionProvider.Create("HotelOwnerVerification")
            .CreateProtector(purpose, user.Id, user.Email);
    }
}