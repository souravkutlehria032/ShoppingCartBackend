using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Utility.DTOs
{
    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = "Contact Info  is must")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 15 digits.")]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        //public IFormFile? ProfilePictureUrl { get; set; }

        public bool TermsAccepted { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [PasswordPropertyText]
        public string Password { get; set; }
    }

    public class VerifyEmailDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }

    public class EmailDto
    {
        public string Email { get; set; }
    }

    public class GoogleLoginDto
    {
        public string Credential { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
    }

    public class FacebookLoginRequest
    {
        public string AccessToken { get; set; }
        public string UserID { get; set; }
    }

    public class FacebookUserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class FacebookLoginDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
    }

    public class LogoutRequest
    {
        public string RefreshToken { get; set; }
        public string DeviceId { get; set; }
    }
}
