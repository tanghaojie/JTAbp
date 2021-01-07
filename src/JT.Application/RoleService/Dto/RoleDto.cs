using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using JT.Abp.Authorization.Roles;
using JT.Authorization.Roles;

namespace JT.RoleService
{
    public class RoleDto : EntityDto<int>
    {
        [Required]
        [StringLength(JTRoleBase.MaxNameLength)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(JTRoleBase.MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        public string NormalizedName { get; set; }

        public List<string> GrantedPermissions { get; set; }
    }
}