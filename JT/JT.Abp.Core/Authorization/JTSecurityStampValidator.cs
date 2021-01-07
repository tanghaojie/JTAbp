using Abp.Domain.Uow;
using JT.Abp.Authorization.Roles;
using JT.Abp.Authorization.Users;
using JT.Abp.MultiTenancy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace JT.Abp.Authorization
{
    public class JTSecurityStampValidator<TTenant, TRole, TUser> : SecurityStampValidator<TUser>
        where TTenant : JTTenant<TUser>
        where TRole : JTRole<TUser>, new()
        where TUser : JTUser<TUser>
    {
        public JTSecurityStampValidator(
            IOptions<SecurityStampValidatorOptions> options,
            JTSignInManager<TTenant, TRole, TUser> signInManager,
            ISystemClock systemClock,
            ILoggerFactory loggerFactory)
            : base(
                options,
                signInManager,
                systemClock,
                loggerFactory)
        {
        }

        [UnitOfWork]
        public override Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            return base.ValidateAsync(context);
        }
    }
}
