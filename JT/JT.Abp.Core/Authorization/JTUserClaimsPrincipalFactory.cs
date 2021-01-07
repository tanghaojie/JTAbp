using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Security;
using JT.Abp.Authorization.Roles;
using JT.Abp.Authorization.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JT.Abp.Authorization
{
    public class JTUserClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser, TRole>, ITransientDependency
        where TRole : JTRole<TUser>, new()
        where TUser : JTUser<TUser>
    {
        public JTUserClaimsPrincipalFactory(
            JTUserManager<TRole, TUser> userManager,
            JTRoleManager<TRole, TUser> roleManager,
            IOptions<IdentityOptions> optionsAccessor
            ) : base(userManager, roleManager, optionsAccessor)
        {
        }

        [UnitOfWork]
        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            var principal = await base.CreateAsync(user);

            if (user.TenantId.HasValue)
            {
                principal.Identities.First().AddClaim(new Claim(AbpClaimTypes.TenantId, user.TenantId.ToString()));
            }

            return principal;
        }
    }
}
