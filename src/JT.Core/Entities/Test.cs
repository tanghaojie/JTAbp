using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;

namespace JT.Entities
{
    public class Test : Entity, IHasCreationTime
    {
        [Required]
        public string Name { get; set; }

        public string XX { get; set; }

        [Required]
        public DateTime CreationTime { get; set; } = DateTime.Now;
    }
}
