using System.Threading.Tasks;
using JT.Abp.Authorization.Users;
using JT.Abp.MultiTenancy;

namespace JT.Abp.Authorization.Users
{
    public interface IExternalAuthenticationSource<TTenant, TUser>
        where TTenant : JTTenant<TUser>
        where TUser : JTUserBase
    {
        string Name { get; }

        Task<bool> TryAuthenticateAsync(string userNameOrEmailAddress, string plainPassword, TTenant tenant);

        Task<TUser> CreateUserAsync(string userNameOrEmailAddress, TTenant tenant);

        Task UpdateUserAsync(TUser user, TTenant tenant);
    }
}
