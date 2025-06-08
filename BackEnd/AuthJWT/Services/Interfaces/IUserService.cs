using AuthJWT.Domain.Contracts;

namespace AuthJWT.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> RegisterUserAsync(UserRegisterRequest userRequest);
        Task<CurrentUserResponse> GetCurrentUserAsync();
        Task<UserProfileResponse> GetByIdAsync(Guid id);
        Task UpdateUserAsync(Guid id, UpdateUserRequest userRequest);
        Task<RevokeRefreshTokenResponse> RevokeRefreshToken(RefreshTokenRequest refreshTokenRequest);
        Task<CurrentUserResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
        Task<UserResponse> LoginAsync(UserLoginRequest userRequest);
        Task ChangePasswordAsync(Guid id, ChangePasswordRequest changePasswordRequest);
        Task UserAvatarAsync(string id, IFormFile file);
        Task<PaginateList<UserDto>> GetAllUsersAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task ChnageStatusAsync(string id, bool status);
        Task DeleteUserAsync(string id);
    }
}