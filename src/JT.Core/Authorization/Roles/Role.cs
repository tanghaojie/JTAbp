using JT.Abp.Authorization.Roles;
using JT.Authorization.Users;

namespace JT.Authorization.Roles
{
    public class Role : JTRole<User>
    {
        public Role()
        {
        }

        public Role(int? tenantId, string displayName)
            : base(tenantId, displayName)
        {
        }

        public Role(int? tenantId, string name, string displayName)
            : base(tenantId, name, displayName)
        {
        }

    }
}
