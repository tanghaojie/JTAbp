using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.Abp.Authorization
{
    public abstract class JTPermission : CreationAuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxNameLength = 128;

        public virtual int? TenantId { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Is this role granted for this permission.
        /// </summary>
        public virtual bool IsGranted { get; set; } = true;

    }
}
