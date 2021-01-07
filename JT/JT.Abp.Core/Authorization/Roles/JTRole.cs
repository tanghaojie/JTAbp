using Abp.Domain.Entities.Auditing;
using JT.Abp.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.Abp.Authorization.Roles
{
    public abstract class JTRole<TUser> : JTRoleBase, IFullAudited<TUser>
        where TUser : JTUser<TUser>
    {
        public const int MaxConcurrencyStampLength = 128;

        [Required]
        [StringLength(MaxNameLength)]
        public virtual string NormalizedName { get; set; }

        [ForeignKey("RoleId")]
        public virtual ICollection<JTRoleClaim> Claims { get; set; }

        public virtual TUser DeleterUser { get; set; }

        public virtual TUser CreatorUser { get; set; }

        public virtual TUser LastModifierUser { get; set; }

        protected JTRole()
        {
            SetNormalizedName();
        }

        protected JTRole(int? tenantId, string displayName)
            : base(tenantId, displayName)
        {
            SetNormalizedName();
        }

        protected JTRole(int? tenantId, string name, string displayName)
            : base(tenantId, name, displayName)
        {
            SetNormalizedName();
        }

        public virtual void SetNormalizedName()
        {
            NormalizedName = Name.ToUpperInvariant();
        }
    }
}
