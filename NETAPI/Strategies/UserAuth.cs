using NETAPI.DTOs;
using NETAPI.Models;
using NETAPI.Repository;
using NETAPI.Services;

namespace NETAPI.Strategies
{
    public class UserAuth : IAuthStrategy
    {
        private readonly JwtService _jwtService;
        private readonly IAuthRepository _repository;
        public UserAuth(JwtService jwtService, IAuthRepository repository)
        {
            _jwtService = jwtService;
            _repository = repository;
        }
        public async Task<AuthResDTO> Authenticate(User user, string password, bool rememberMe)
        {
            if (!BCrypt.Net.BCrypt.Verify(password, user.Hash))
                throw new UnauthorizedAccessException("Invalid credentials");

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var expireDays = rememberMe ? 30 : 1;

            var token = new Token
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                ExpiresIn = DateTime.UtcNow.AddDays(expireDays).ToString("o"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddRefreshTokenAsync(token);
            await _repository.SaveChangesAsync();

            return new AuthResDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
