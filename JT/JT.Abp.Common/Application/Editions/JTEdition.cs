using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using System;
using System.ComponentModel.DataAnnotations;

namespace JT.Abp.Application.Editions
{
    [MultiTenancySide(MultiTenancySides.Host)]
    public class JTEdition : FullAuditedEntity
    {
        public const int MaxNameLength = 32;

        public const int MaxDisplayNameLength = 64;

        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        public JTEdition()
        {
            Name = Guid.NewGuid().ToString("N");
        }

        public JTEdition(string displayName)
            : this()
        {
            DisplayName = displayName;
        }
    }
}
