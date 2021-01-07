using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.Abp.Authorization.Roles
{
    [Serializable]
    public class JTRolePermissionCacheItem
    {
        public const string CacheStoreName = "JTRolePermissions";

        public long RoleId { get; set; }

        public HashSet<string> GrantedPermissions { get; set; }

        public JTRolePermissionCacheItem()
        {
            GrantedPermissions = new HashSet<string>();
        }

        public JTRolePermissionCacheItem(int roleId)
            : this()
        {
            RoleId = roleId;
        }
    }
}
