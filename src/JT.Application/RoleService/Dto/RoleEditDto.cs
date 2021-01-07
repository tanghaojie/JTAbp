using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using JT.Abp.Authorization.Roles;

namespace JT.RoleService
{
    public class RoleEditDto: EntityDto<int>
    {
        [Required]
        [StringLength(JTRoleBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(JTRoleBase.MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        public bool IsStatic { get; set; }
    }
}