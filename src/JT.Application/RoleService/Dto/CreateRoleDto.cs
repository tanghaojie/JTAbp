using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JT.Abp.Authorization.Roles;

namespace JT.RoleService
{
    public class CreateRoleDto
    {
        [Required]
        [StringLength(JTRoleBase.MaxNameLength)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(JTRoleBase.MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        public string NormalizedName { get; set; }

        public List<string> GrantedPermissions { get; set; }

        public CreateRoleDto()
        {
            GrantedPermissions = new List<string>();
        }
    }
}
