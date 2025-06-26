using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.Entities.Security;
using AuthJWT.Exceptions;
using AuthJWT.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Services.Implements
{
    /// <summary>
    /// /// UserService class implements IUserService interface to handle user-related operations.
    /// /// It provides methods for user registration, login, token generation, and user management.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IS3Service _s3Service;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IS3Service s3Service, ITokenService tokenService, ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager, IMapper mapper, ILogger<UserService> logger, RoleManager<IdentityRole> roleManager)
        {
            _s3Service = s3Service;
            _tokenService = tokenService;
            _roleManager = roleManager;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task ChangePasswordAsync(Guid id,ChangePasswordRequest changePasswordRequest)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogInformation("User not found");
                throw new Exception("User not found");
            }
            if (changePasswordRequest.OldPassword == changePasswordRequest.NewPassword)
            {
                _logger.LogInformation("New password must be different from old password");
                throw new Exception("New password must be different from old password");
            }
            if (changePasswordRequest.NewPassword != changePasswordRequest.ConfirmPassword)
            {
                _logger.LogInformation("New password and confirm password do not match");
                throw new Exception("New password and confirm password do not match");
            }
            
            var result = await _userManager.ChangePasswordAsync(user, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogInformation("Failed to change password: {errors}", errors);
                throw new Exception($"Failed to change password : {errors}");
            }
        }

        public async Task ChnageStatusAsync(string id, bool status)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogInformation("User not found");
                throw new Exception("User not found");
            }

            user.isActive = status;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogInformation("Failed to update user status: {errors}", errors);
                throw new Exception($"Failed to update user status: {errors}");
            }

            _logger.LogInformation("User status updated successfully");
        }


        public async Task DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogInformation("User not found");
                throw new Exception("User not found");
            }
            var result = await _userManager.DeleteAsync(user);
            var avatar = user.Avatar;
            if (avatar != null)
            {
                await _s3Service.DeleteFileAsync(avatar);
            }
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogInformation("Failed to delete user: {errors}", errors);
                throw new Exception($"Failed to delete user : {errors}");
            }
        }

        public async Task<PaginateList<UserDto>> GetAllUsersAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var usersQuery = _userManager.Users.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                usersQuery = usersQuery.Where(u => u.FirstName.Contains(searchTerm) || u.LastName.Contains(searchTerm) || u.Email.Contains(searchTerm));
            }

            var users = await usersQuery.ToListAsync();
            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                if (user.Avatar != null)
                {
                    var fileUrl = await _s3Service.GetFileUrlAsync(user.Avatar);
                    user.Avatar = fileUrl.ToString();
                }
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth ?? DateTime.MinValue,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Location = user.Location ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Avatar = user.Avatar ?? string.Empty,
                    IsActive = user.isActive,
                    RoleName = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "User"
                });

            }
            return PaginateList<UserDto>.Create(userDtos, pageNumber, pageSize);
        }

        public async Task<UserProfileResponse> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting user by id: {id}", id);
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogInformation("User not found");
                throw new ResourceNotFoundException("User not found");
            }
            if (user.Avatar != null)
            {
                var fileUrl = await _s3Service.GetFileUrlAsync(user.Avatar);
                user.Avatar = fileUrl.ToString();
            }

            return _mapper.Map<UserProfileResponse>(user);
        }

        public async Task<CurrentUserResponse> GetCurrentUserAsync()
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.GetUserId());
            if (user == null)
            {
                _logger.LogInformation("User not found");
                throw new Exception("User not found");
            }
            return _mapper.Map<CurrentUserResponse>(user);
        }

        public async Task<UserResponse> LoginAsync(UserLoginRequest userRequest)
        {
            if (userRequest == null)
            {
                _logger.LogError("User request is null");
                throw new ArgumentNullException(nameof(userRequest));
            }

            var user = await _userManager.FindByEmailAsync(userRequest.Email);

            if (user == null)
            {
                _logger.LogInformation("Invalid email");
                throw new Exception("Invalid email");
            }
            if (!await _userManager.CheckPasswordAsync(user, userRequest.Password)) throw new Exception("Invalid password");
            //generate access token
            if(user.isActive == false)
            {
                _logger.LogInformation("User is not active");
                throw new Exception("User is not active");
            }
            var token = await _tokenService.GenerateToken(user);

            var refreshToken = _tokenService.GenerateRefreshToken();
            using var sha256 = SHA256.Create();
            var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
            var refreshTokenString = Convert.ToBase64String(refreshTokenHash);
            user.RefreshToken = refreshTokenString;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(6);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogInformation("Failed to update refresh token: {errors}", errors);
                throw new Exception($"Failed to update refresh token : {errors}");
            }

            return new UserResponse
            {
                AccessToken = token,
                RefreshToken = refreshTokenString
            };
        }

        public async Task<CurrentUserResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            _logger.LogInformation("Verifying access token and refresh token");


            // Find user by refresh token
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshTokenRequest.RefreshToken);
            if (user == null)
            {
                _logger.LogInformation("Invalid refresh token");
                throw new Exception("Invalid refresh token");
            }
            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                _logger.LogInformation("Refresh token expired");
                throw new Exception("Refresh token expired");
            }


            var principal = _tokenService.GetPrincipalFromExpiredToken(refreshTokenRequest.AccessToken);
            if (principal == null)
            {
                _logger.LogInformation("Invalid access token");
                throw new Exception("Invalid access token");
            }

            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || userIdClaim != user.Id)
            {
                _logger.LogInformation("Access token and refresh token don't match");
                throw new Exception("Access token and refresh token don't match");
            }
            

            var newAccessToken = await _tokenService.GenerateToken(user);
            _logger.LogInformation("Token verified and refreshed successfully");
            var currentUserRes = _mapper.Map<CurrentUserResponse>(user);
            currentUserRes.AccessToken = newAccessToken;
            return currentUserRes;

        }
        /** 
        * Register a new user
        * @param userRequest
        * @return UserResponse
        */
        public async Task<UserResponse> RegisterUserAsync(UserRegisterRequest userRequest)
        {
            _logger.LogInformation("Registering user");
            var user = await _userManager.FindByEmailAsync(userRequest.Email);
            if (user != null)
            {
                _logger.LogInformation("User already exists");
                throw new Exception("User already exists");
            }
            var newUser = _mapper.Map<ApplicationUser>(userRequest);
            var userName = $"{userRequest.FirstName}{userRequest.LastName}".Replace(" ", "") + Guid.NewGuid().ToString("N")[..8];
            newUser.UserName = userName;
            var result = await _userManager.CreateAsync(newUser, userRequest.Password);

            await _userManager.AddToRoleAsync(newUser, "User");
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogInformation("Failed to create user: {errors}", errors);
                throw new Exception($"Failed to create user : {errors}");
            }
            _logger.LogInformation("User created successfully");
            return _mapper.Map<UserResponse>(newUser);
        }


        public async Task<RevokeRefreshTokenResponse> RevokeRefreshToken(RefreshTokenRequest refreshTokenRequest)
        {

            try
            {
                using var sha256 = SHA256.Create();
                var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshTokenRequest.RefreshToken));
                var refreshToken = Convert.ToBase64String(refreshTokenHash);

                var user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
                if (user == null)
                {
                    _logger.LogInformation("Invalid refresh token");
                    throw new Exception("Invalid refresh token");
                }
                if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
                {
                    _logger.LogInformation("Refresh token expired");
                    throw new Exception("Refresh token expired");
                }
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                var res = await _userManager.UpdateAsync(user);
                if (!res.Succeeded)
                {
                    _logger.LogError("Failed to revoke refresh token");
                    {
                        return new RevokeRefreshTokenResponse
                        {
                            Message = "Failed to revoke refresh token"
                        };
                    }
                }
                return new RevokeRefreshTokenResponse
                {
                    Message = "Refresh token revoked successfully"
                };

            }
            catch (Exception)
            {

                _logger.LogWarning("Failed to revoke refresh token");
                throw new Exception("Failed to revoke refresh token");
            }
        }

        public async Task UpdateUserAsync(Guid id, UpdateUserRequest userRequest)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogInformation("User not found");
                throw new Exception("User not found");
            }

            // Use AutoMapper to map the update request to the user entity
            _mapper.Map(userRequest, user);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogInformation("Failed to update user: {errors}", errors);
                throw new Exception($"Failed to update user : {errors}");
            }
            _logger.LogInformation("User updated successfully");

        }


        public async Task UserAvatarAsync(string id, IFormFile file)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogInformation("User not found");
                throw new Exception("User not found");
            }

            if (file != null)
            {
                if (user.Avatar != null)
                {
                    await _s3Service.DeleteFileAsync(user.Avatar);
                }
                var fileName = await _s3Service.UploadFileAsync(file);
                user.Avatar = fileName.ToString();
                await _userManager.UpdateAsync(user);
            }

        }
    }
}