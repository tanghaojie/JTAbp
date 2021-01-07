using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using System.ComponentModel.DataAnnotations;

namespace JT.Abp.MultiTenancy
{
    [MultiTenancySide(MultiTenancySides.Host)]
    public abstract class JTTenantBase : FullAuditedEntity<int>, IPassivable
    {
        public const int MaxTenancyNameLength = 64;

        public const int MaxConnectionStringLength = 1024;

        public const string DefaultTenantName = "Default";

        public const string TenancyNameRegex = "^[a-zA-Z][a-zA-Z0-9_-]{1,}$";

        public const int MaxNameLength = 128;

        [Required]
        [StringLength(MaxTenancyNameLength)]
        public virtual string TenancyName { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        [StringLength(MaxConnectionStringLength)]
        public virtual string ConnectionString { get; set; }

        public virtual bool IsActive { get; set; }
    }
}
