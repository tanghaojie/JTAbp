using JT.Abp.MultiTenancy;
using System.Threading.Tasks;

namespace JT.Abp.Authorization.Users
{
    public abstract class JTExternalAuthenticationSource<TTenant, TUser> : IExternalAuthenticationSource<TTenant, TUser>
        where TTenant : JTTenant<TUser>
        where TUser : JTUserBase, new()
    {
        public abstract string Name { get; }

        public abstract Task<bool> TryAuthenticateAsync(string userNameOrEmailAddress, string plainPassword, TTenant tenant);

        public virtual Task<TUser> CreateUserAsync(string userNameOrEmailAddress, TTenant tenant)
        {
            return Task.FromResult(
                new TUser
                {
                    UserName = userNameOrEmailAddress,
                    Name = userNameOrEmailAddress,
                    Surname = userNameOrEmailAddress,
                    EmailAddress = userNameOrEmailAddress,
                    IsEmailConfirmed = true,
                    IsActive = true
                });
        }

        public virtual Task UpdateUserAsync(TUser user, TTenant tenant)
        {
            return Task.FromResult(0);
        }
    }
}
