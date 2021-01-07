using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JT.Abp.Configuration
{
    public class JTSetting : AuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxNameLength = 256;

        public virtual int? TenantId { get; set; }

        public virtual long? UserId { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        public virtual string Value { get; set; }

        public JTSetting()
        {

        }

        public JTSetting(int? tenantId, long? userId, string name, string value)
        {
            TenantId = tenantId;
            UserId = userId;
            Name = name;
            Value = value;
        }
    }
}
