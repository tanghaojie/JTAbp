using Abp;
using Abp.Domain.Entities;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.Abp.Authorization.Users
{
    public class JTUserToken : Entity<long>, IMayHaveTenant
    {
        public const int MaxLoginProviderLength = 128;

        public const int MaxNameLength = 128;

        public const int MaxValueLength = 512;

        public virtual int? TenantId { get; set; }

        public virtual long UserId { get; set; }

        [StringLength(MaxLoginProviderLength)]
        public virtual string LoginProvider { get; set; }

        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        [StringLength(MaxValueLength)]
        public virtual string Value { get; set; }

        public virtual DateTime? ExpireDate { get; set; }

        protected JTUserToken()
        {

        }

        protected internal JTUserToken(JTUserBase user, [NotNull] string loginProvider, [NotNull] string name, string value, DateTime? expireDate = null)
        {
            Check.NotNull(loginProvider, nameof(loginProvider));
            Check.NotNull(name, nameof(name));

            TenantId = user.TenantId;
            UserId = user.Id;
            LoginProvider = loginProvider;
            Name = name;
            Value = value;
            ExpireDate = expireDate;
        }
    }
}
