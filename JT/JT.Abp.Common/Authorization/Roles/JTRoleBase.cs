using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JT.Abp.Authorization.Roles
{
    public abstract class JTRoleBase : FullAuditedEntity<int>, IMayHaveTenant
    {
        public const int MaxDisplayNameLength = 64;

        public const int MaxNameLength = 32;

        public virtual int? TenantId { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Static roles can not be deleted, can not change their name.
        /// They can be used programmatically.
        /// </summary>
        public virtual bool IsStatic { get; set; }

        public virtual bool IsDefault { get; set; }

        [ForeignKey("RoleId")]
        public virtual ICollection<JTRolePermission> Permissions { get; set; }

        protected JTRoleBase()
        {
            Name = Guid.NewGuid().ToString("N");
        }

        protected JTRoleBase(int? tenantId, string displayName)
            : this()
        {
            TenantId = tenantId;
            DisplayName = displayName;
        }

        protected JTRoleBase(int? tenantId, string name, string displayName)
            : this(tenantId, displayName)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"[Role {Id}, Name={Name}]";
        }
    }
}
