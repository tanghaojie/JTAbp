using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace JT.Abp.Authorization.Roles
{
    public class JTRoleClaim : CreationAuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxClaimTypeLength = 256;

        public virtual int? TenantId { get; set; }

        public virtual int RoleId { get; set; }

        [StringLength(MaxClaimTypeLength)]
        public virtual string ClaimType { get; set; }

        public virtual string ClaimValue { get; set; }

        public JTRoleClaim()
        {
        }

        public JTRoleClaim(JTRoleBase role, Claim claim)
        {
            TenantId = role.TenantId;
            RoleId = role.Id;
            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }
    }
}
