using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Utility.Services
{
    public class JwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryMinutes;

        public JwtService(IConfiguration configuration)
        {
            _secretKey = configuration["JwtSettings:SecretKey"]
                ?? throw new ArgumentNullException(nameof(_secretKey), "JWT Secret Key is missing in the configuration.");

            _issuer = configuration["JwtSettings:Issuer"]
                ?? throw new ArgumentNullException(nameof(_issuer), "JWT Issuer is missing in the configuration.");

            _audience = configuration["JwtSettings:Audience"]
                ?? throw new ArgumentNullException(nameof(_audience), "JWT Audience is missing in the configuration.");

            var expiryMinutes = configuration["JwtSettings:ExpiryMinutes"];

            _expiryMinutes = expiryMinutes != null
                ? int.Parse(expiryMinutes)
                : throw new ArgumentNullException(nameof(_expiryMinutes), "JWT ExpiryMinutes is missing in the configuration.");
        }

        public string GenerateToken(string Id, string Role, string UserName, string deviceId)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    throw new ArgumentNullException(nameof(Id));

                if (string.IsNullOrEmpty(Role))
                    Role = "User";

                if (string.IsNullOrEmpty(UserName))
                    UserName = "Unknown";

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, Role),
                    new Claim(ClaimTypes.Name, UserName),
                    new Claim("deviceId", deviceId)
                };

                var keyBytes = Encoding.UTF8.GetBytes(
                    _secretKey ?? throw new InvalidOperationException("JWT Secret Key is null"));

                var key = new SymmetricSecurityKey(keyBytes);
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to generate JWT token. See inner exception.", ex);
            }
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public (string Token, string RefreshToken, DateTime RefreshTokenExpiry)
            GenerateTokenPair(
                string Id,
                string deviceId,
                string Role,
                string UserName,
                bool rememberMe = false)
        {
            var token = GenerateToken(Id, Role, UserName, deviceId);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenExpiry = rememberMe
                ? DateTime.UtcNow.AddDays(30)
                : DateTime.UtcNow.AddDays(7);

            return (token, refreshToken, refreshTokenExpiry);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _audience,

                ValidateIssuer = true,
                ValidIssuer = _issuer,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_secretKey)),

                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(
                token,
                tokenValidationParameters,
                out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
