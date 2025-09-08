using NETAPI.Models;

namespace NETAPI.Strategies
{
    public class AuthStrategy
    {
        private readonly IServiceProvider _provider;

        public AuthStrategy(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IAuthStrategy GetStrategy(UserType type)
        {
            if (type == UserType.User)
                return _provider.GetRequiredService<UserAuth>();

            if (type == UserType.Admin)
                return _provider.GetRequiredService<AdminAuth>();

            throw new NotImplementedException("Unsupported user type");
        }
    }
}
