using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace JT.Abp.Authorization.Users
{
    public class JTUserRole : CreationAuditedEntity<long>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }

        public virtual long UserId { get; set; }

        public virtual int RoleId { get; set; }

        public JTUserRole()
        {
        }

        public JTUserRole(int? tenantId, long userId, int roleId)
        {
            TenantId = tenantId;
            UserId = userId;
            RoleId = roleId;
        }
    }
}
