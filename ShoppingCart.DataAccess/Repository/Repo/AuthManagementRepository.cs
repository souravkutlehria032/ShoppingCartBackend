using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShoppingCart.DataAccess.Data;
using ShoppingCart.DataAccess.Entity;
using ShoppingCart.DataAccess.Repository.IRepo;
using ShoppingCart.Utility.DTOs;
using ShoppingCart.Utility.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository.Repo
{
    public class AuthManagementRepository : IAuthManagementRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthManagementRepository> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtService _jwtService;

        private readonly ConcurrentDictionary<string,
            (ApplicationUser user, string password, string role, string otp, DateTime CreatedAt)>
            _temporaryUserStore;

        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly LinkGenerator _linkGenerator;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthManagementRepository(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<AuthManagementRepository> logger,
            LinkGenerator linkGenerator,
            IHttpContextAccessor httpContextAccessor,
            SignInManager<ApplicationUser> signInManager,
            JwtService jwtService,
            ConcurrentDictionary<string,
                (ApplicationUser user, string password, string role, string otp, DateTime CreatedAt)>
                temporaryUserStore,
            IEmailService emailService,
            IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;
            _temporaryUserStore = temporaryUserStore;
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _hostEnvironment = hostEnvironment;
        }

        public Task<ReturnMessageDto> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> ConfirmEmail(VerifyEmailDto verifyEmail)
        {
            throw new NotImplementedException();
        }

        public Task<(bool Succeeded, string Message, string Token, string RefreshToken, DateTime RefreshTokenExpiry, ApplicationUser User, string Role)> FacebookLoginAsync(string accessToken, string userId, string deviceId, IConfiguration config)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FindOrCreateExternalUser(string provider, string providerKey, string email, string name)
        {
            throw new NotImplementedException();
        }

        public string GenerateJwtToken(string userId, string role, string username, string deviceId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetUserRoles(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<(bool Succeeded, string Message, string Token, string RefreshToken, DateTime RefreshTokenExpiry, ApplicationUser User, string Role)> GoogleLoginAsync(string credential, string deviceId)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> Login(string email, string password, string deviceId, bool rememberMe = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogoutAsync(string refreshToken, string deviceInfo = "default")
        {
            throw new NotImplementedException();
        }

        public Task<(string accessToken, string refreshToken, DateTime refreshTokenExpiry)> RefreshTokenAsync(string refreshToken, string deviceId)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> Register(RegisterDto registerDto)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> ResendOtpAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> ResetPassword(ResetPasswordDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnMessageDto> SendResetPasswordEmail(SendEmailResetDto sendEmailReset)
        {
            throw new NotImplementedException();
        }
    }
}
