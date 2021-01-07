using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using System.ComponentModel.DataAnnotations;

namespace JT.Abp.Application.Features
{
    [MultiTenancySide(MultiTenancySides.Host)]
    public abstract class JTFeature : CreationAuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxNameLength = 128;

        public const int MaxValueLength = 2000;

        public virtual int? TenantId { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(MaxValueLength)]
        public virtual string Value { get; set; }

        protected JTFeature()
        {
        }

        protected JTFeature(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
