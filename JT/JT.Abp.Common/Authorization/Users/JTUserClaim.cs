using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace JT.Abp.Authorization.Users
{
    public class JTUserClaim : CreationAuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxClaimTypeLength = 256;

        public virtual int? TenantId { get; set; }

        public virtual long UserId { get; set; }

        [StringLength(MaxClaimTypeLength)]
        public virtual string ClaimType { get; set; }

        public virtual string ClaimValue { get; set; }

        public JTUserClaim()
        {
        }

        public JTUserClaim(JTUserBase user, Claim claim)
        {
            TenantId = user.TenantId;
            UserId = user.Id;
            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }
    }
}
