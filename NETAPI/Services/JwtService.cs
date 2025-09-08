using Microsoft.IdentityModel.Tokens;
using NETAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NETAPI.Services
{
    public class JwtService
    {
        private readonly string _secret;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _accessTokenExpr;

        public JwtService(IConfiguration config)
        {
            _secret = config["Jwt:Secret"];
            _issuer = config["Jwt:Issuer"];
            _audience = config["Jwt:Audience"];
            int tokenExpr;
            int.TryParse(config["Jwt:AccessTokenExpr"], out tokenExpr);
            _accessTokenExpr = tokenExpr;
        }

        public virtual string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_accessTokenExpr),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public virtual string GenerateRefreshToken()
        {
            var randomBytes = new byte[192];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            return Convert.ToBase64String(randomBytes).Substring(0, 250);
        }
    }
}
