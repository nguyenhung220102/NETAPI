using NETAPI.Models;

namespace NETAPI.Repository
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task SaveChangesAsync();

        Task<Token?> GetRefreshTokenAsync(string refreshToken);
        Task AddRefreshTokenAsync(Token token);
        Task RemoveAllRefreshTokensAsync(Token token);
        Task<User> GetUserByIdAsync(int userId);
        Task RemoveRefreshTokenAsync(Token token);
    }
}