using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.Abp.Authorization.Users
{
    public class JTUserLoginAttempt : Entity<long>, IHasCreationTime, IMayHaveTenant
    {
        public const int MaxTenancyNameLength = 64; // AbpTenantBase.MaxTenancyNameLength;

        public const int MaxUserNameOrEmailAddressLength = JTUserBase.MaxEmailAddressLength;

        public const int MaxClientIpAddressLength = 64;

        public const int MaxClientNameLength = 128;

        public const int MaxBrowserInfoLength = 512;

        public virtual int? TenantId { get; set; }

        [StringLength(MaxTenancyNameLength)]
        public virtual string TenancyName { get; set; }

        public virtual long? UserId { get; set; }

        [StringLength(MaxUserNameOrEmailAddressLength)]
        public virtual string UserNameOrEmailAddress { get; set; }

        [StringLength(MaxClientIpAddressLength)]
        public virtual string ClientIpAddress { get; set; }

        [StringLength(MaxClientNameLength)]
        public virtual string ClientName { get; set; }

        [StringLength(MaxBrowserInfoLength)]
        public virtual string BrowserInfo { get; set; }

        public virtual JTLoginResultType Result { get; set; }

        public virtual DateTime CreationTime { get; set; } = DateTime.Now;
    }
}
