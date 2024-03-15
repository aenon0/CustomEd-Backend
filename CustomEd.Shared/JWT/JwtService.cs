// Purpose: The JwtService class is responsible for generating and validating JWT tokens. It provides methods to generate a JWT token based on user information and to validate the authenticity and expiration of a given token.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomEd.Shared.JWT.Contracts;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CustomEd.Shared.JWT
{
    /// <summary>
    /// The JwtService class is responsible for generating and validating JWT tokens.
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly JWTSettings _jwtSettings;

        /// <summary>
        /// Initializes a new instance of the JwtService class.
        /// </summary>
        /// <param name="jwtSettings">The JWT settings.</param>
        public JwtService(IOptions<JWTSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Generates a JWT token based on the provided user information.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <returns>The generated JWT token.</returns>
        public string GenerateToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim(JwtRegisteredClaimNames.Aud, _jwtSettings.Issuer),
                    new Claim(JwtRegisteredClaimNames.Iss, _jwtSettings.Issuer)
                }),

                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Validates the authenticity and expiration of the provided JWT token.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>True if the token is valid; otherwise, false.</returns>
        public bool IsTokenValid(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Issuer,
                    ValidateLifetime = true
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
