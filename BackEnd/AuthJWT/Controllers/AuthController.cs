using AuthJWT.Domain.Contracts;
using AuthJWT.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthJWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IHotelOwnerService _hotelOwnerService;

        public AuthController(IUserService userService, IHotelOwnerService hotelOwnerService)
        {
            _userService = userService;
            _hotelOwnerService = hotelOwnerService;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            var response = await _userService.RegisterUserAsync(request);
            return Ok(response);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            try
            {
                var response = await _userService.LoginAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
        [HttpGet("user/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var response = await _userService.GetByIdAsync(id);
            return Ok(response);
        }
        [HttpPost("refresh-token")]
        [Authorize(Roles = "User,Admin,HotelOwner")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _userService.RefreshTokenAsync(request);
            return Ok(response);
        }

        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _userService.RevokeRefreshToken(request);
            if (response != null && response.Message == "Refresh token revoked successfully")
            {
                return Ok(response);

            }
            return BadRequest(response);
        }
        [HttpGet("current-user")]
        [Authorize(Roles = "User,Admin,HotelOwner")]

        public async Task<IActionResult> GetCurrentUser()
        {
            var response = await _userService.GetCurrentUserAsync();
            return Ok(response);
        }

        [HttpPut("update-user/{id}")]
        [Authorize(Roles = "User,Admin,HotelOwner")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            await _userService.UpdateUserAsync(id, request);
            return Ok();
        }
        [HttpPost("update-avatar/{id}")]
        [Authorize(Roles = "User,Admin,HotelOwner")]
        public async Task<IActionResult> UpdateAvatar(string id, IFormFile file)
        {
            await _userService.UserAvatarAsync(id, file);
            return Ok();
        }
        [HttpPost("change-password/{id}")]
        [Authorize(Roles = "User,Admin,HotelOwner")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request)
        {
            await _userService.ChangePasswordAsync(id, request);
            return Ok();
        }
        [HttpPost("generate-hotel-owner-token")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GenerateHotelOwnerToken(string email)
        {
            try
            {
                var token = await _hotelOwnerService.GenerateHotelOwnerTokenAsync(email);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("verify-token")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> VerifyHotelOwnerToken(string userId, string token)
        {
            try
            {
                var isValid = await _hotelOwnerService.VerifyHotelOwnerTokenAsync(userId, token);
                return Ok(new { IsValid = isValid });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}