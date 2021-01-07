using JT.Abp.MultiTenancy;
using System.Security.Claims;

namespace JT.Abp.Authorization.Users
{
    public class JTLoginResult<TTenant, TUser>
          where TTenant : JTTenant<TUser>
        where TUser : JTUserBase
    {
        public JTLoginResultType Result { get; private set; }

        public TTenant Tenant { get; private set; }

        public TUser User { get; private set; }

        public ClaimsIdentity Identity { get; private set; }

        public JTLoginResult(JTLoginResultType result, TTenant tenant = null, TUser user = null)
        {
            Result = result;
            Tenant = tenant;
            User = user;
        }

        public JTLoginResult(TTenant tenant, TUser user, ClaimsIdentity identity)
            : this(JTLoginResultType.Success, tenant)
        {
            User = user;
            Identity = identity;
        }
    }
}
