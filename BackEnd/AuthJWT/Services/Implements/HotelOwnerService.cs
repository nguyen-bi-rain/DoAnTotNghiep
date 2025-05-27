using System.Net;
using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.Entities.Security;
using AuthJWT.Exceptions;
using AuthJWT.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthJWT.Services.Implements
{
    public class HotelOwnerService : IHotelOwnerService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISendEmailService _emailService;

        public HotelOwnerService(UserManager<ApplicationUser> userManager, ISendEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<string> GenerateHotelOwnerTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new ResourceNotFoundException("User not found");

            if (user.isHotelOwner)
                throw new InvalidOperationException("User is already a hotel owner");

            // Generate token
            var token = await _userManager.GenerateUserTokenAsync(
                user,
                "HotelOwnerTokenProvider",
                "VerifyHotelOwner");

            // Send email with verification link
            await SendHotelOwnerVerificationEmailAsync(user, token);

            return token;
        }

        public async Task<bool> VerifyHotelOwnerTokenAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ResourceNotFoundException("User not found");

            if (user.isHotelOwner)
                return true; // Already verified

            // Validate token
            var isValid = await _userManager.VerifyUserTokenAsync(
                user,
                "HotelOwnerTokenProvider",
                "VerifyHotelOwner",
                token);

            if (isValid)
            {
                // Set user as hotel owner
                user.isHotelOwner = true;
                await _userManager.RemoveFromRoleAsync(user, "User");
                await _userManager.AddToRoleAsync(user, "HotelOwner");
                await _userManager.UpdateAsync(user);

                // Send confirmation email
                await SendHotelOwnerConfirmationEmailAsync(user);
            }

            return isValid;
        }

        private async Task SendHotelOwnerVerificationEmailAsync(ApplicationUser user, string token)
        {
            var verificationLink = $"http://localhost:3000/verify-token?userId={user.Id}&token={WebUtility.UrlEncode(token)}";

            var emailBody = $@"
        <h2>Hotel Owner Verification</h2>
        <p>Hello {user.FirstName} {user.LastName},</p>
        <p>Please click the link below to verify your account as a hotel owner:</p>
        <p><a href='{verificationLink}'>Verify as Hotel Owner</a></p>
        <p>If you didn't request this, you can safely ignore this email.</p>
        <p>Thank you,<br>The Hotel Booking Team</p>";

            var emaildata = new MailRequest
            {
                ToEmail = user.Email,
                Subject = "Verify Your Hotel Owner Account",
                Body = emailBody
            };
            await _emailService.SendEmailAsync(emaildata);
        }

        private async Task SendHotelOwnerConfirmationEmailAsync(ApplicationUser user)
        {
            var emailBody = $@"
        <h2>Hotel Owner Verification Complete</h2>
        <p>Hello {user.FirstName} {user.LastName},</p>
        <p>Your account has been successfully verified as a hotel owner!</p>
        <p>You can now add hotels and manage your properties on our platform.</p>
        <p>Thank you,<br>The Hotel Booking Team</p>";

            await _emailService.SendEmailAsync(new MailRequest
            {
                ToEmail = user.Email,
                Subject = "Hotel Owner Verification Complete",
                Body = emailBody
            });
        }
    }
}