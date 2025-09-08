using NETAPI.Models;
using NETAPI.Repository;
using NETAPI.Data;

using Microsoft.EntityFrameworkCore;

namespace NETAPI.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Token> GetRefreshTokenAsync(string refreshToken)
        {
            return await _context.Tokens.Include(t => t.User)
                        .FirstOrDefaultAsync(t => t.RefreshToken == refreshToken);
        }

        public async Task AddRefreshTokenAsync(Token token)
        {
            await _context.Tokens.AddAsync(token);
        }
        public async Task RemoveRefreshTokenAsync(Token token)
        {
            _context.Tokens.Remove(token);
        }
        public async Task RemoveAllRefreshTokensAsync(Token token)
        {
            var tokens = await _context.Tokens.Where(t => t.UserId == token.UserId).ToListAsync();
            _context.Tokens.RemoveRange(tokens);
        }
    }
}
