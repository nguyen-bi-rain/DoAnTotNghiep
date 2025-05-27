using AuthJWT.Domain.Contracts;

namespace AuthJWT.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> RegisterUserAsync(UserRegisterRequest userRequest);
        Task<CurrentUserResponse> GetCurrentUserAsync();
        Task<UserProfileResponse> GetByIdAsync(Guid id);
        Task UpdateUserAsync(Guid id, UpdateUserRequest userRequest);
        Task DeleteUserAsync(Guid id);
        Task<RevokeRefreshTokenResponse> RevokeRefreshToken(RefreshTokenRequest refreshTokenRequest);
        Task<CurrentUserResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
        Task<UserResponse> LoginAsync(UserLoginRequest userRequest);
        Task ChangePasswordAsync(Guid id,ChangePasswordRequest changePasswordRequest);
        Task UserAvatarAsync(string id, IFormFile file);

    }
}