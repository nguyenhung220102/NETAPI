using NETAPI.DTOs;
using NETAPI.Models;

namespace NETAPI.Strategies
{
    public interface IAuthStrategy
    {
        Task<AuthResDTO> Authenticate(User user, string password, bool rememberMe);
    }
}
