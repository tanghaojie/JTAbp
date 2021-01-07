using Abp.Domain.Entities.Auditing;
using JT.Abp.Application.Editions;
using JT.Abp.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.Abp.MultiTenancy
{
    public abstract class JTTenant<TUser> : JTTenantBase, IFullAudited<TUser>
        where TUser : JTUserBase
    {
        public virtual JTEdition Edition { get; set; }
        public virtual int? EditionId { get; set; }

        public virtual TUser CreatorUser { get; set; }

        public virtual TUser LastModifierUser { get; set; }

        public virtual TUser DeleterUser { get; set; }

        protected JTTenant()
        {
            IsActive = true;
        }

        protected JTTenant(string tenancyName, string name)
            : this()
        {
            TenancyName = tenancyName;
            Name = name;
        }
    }
}
