namespace AuthJWT.Domain.Contracts
{
    public class UserRegisterRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserResponse
    {
        public string AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }

    public class UserLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class CurrentUserResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string AccessToken { get; set; }
    }
    public class UpdateUserRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Location { get; set; }
        public string? IdentifyNumber { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }

    }

    public class RevokeRefreshTokenResponse
    {
        public string Message { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }

    public class UserProfileResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
        public string IdentifyNumber { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string Gender { get; set; }
    }
}

public class ChangePasswordRequest
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}