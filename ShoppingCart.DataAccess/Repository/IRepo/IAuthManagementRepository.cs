using Microsoft.Extensions.Configuration;
using ShoppingCart.DataAccess.Entity;
using ShoppingCart.Utility.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository.IRepo
{
    public interface IAuthManagementRepository
    {
        Task<ReturnMessageDto> Login(string email, string password, string deviceId, bool rememberMe = false);

        Task<(string accessToken, string refreshToken, DateTime refreshTokenExpiry)> RefreshTokenAsync(
            string refreshToken, string deviceId);

        Task<bool> LogoutAsync(string refreshToken, string deviceInfo = "default");

        Task<ReturnMessageDto> ResetPassword(ResetPasswordDto dto);

        Task<ReturnMessageDto> Register(RegisterDto registerDto);

        Task<ReturnMessageDto> ConfirmEmail(VerifyEmailDto verifyEmail);

        Task<ReturnMessageDto> SendResetPasswordEmail(SendEmailResetDto sendEmailReset);

        Task<ReturnMessageDto> ResendOtpAsync(string email);

        Task<(bool Succeeded, string Message, string Token, string RefreshToken,
            DateTime RefreshTokenExpiry, ApplicationUser User, string Role)>
            FacebookLoginAsync(
                string accessToken,
                string userId,
                string deviceId,
                IConfiguration config);

        Task<(bool Succeeded, string Message, string Token, string RefreshToken,
            DateTime RefreshTokenExpiry, ApplicationUser User, string Role)>
            GoogleLoginAsync(string credential, string deviceId);

        Task<IList<string>> GetUserRoles(ApplicationUser user);

        string GenerateJwtToken(string userId, string role, string username, string deviceId);

        Task<ReturnMessageDto> ChangePasswordAsync(
            string userId,
            string oldPassword,
            string newPassword);

        Task<ApplicationUser> FindOrCreateExternalUser(
            string provider,
            string providerKey,
            string email,
            string name);
    }
}
