using System;
using System.Collections.Generic;

namespace JT.Abp.Common.Authorization.Users
{
    [Serializable]
    public class JTUserPermissionCacheItem
    {
        public const string CacheStoreName = "JTUserPermissions";

        public long UserId { get; set; }

        public List<int> RoleIds { get; set; }

        public HashSet<string> GrantedPermissions { get; set; }

        public HashSet<string> ProhibitedPermissions { get; set; }

        public JTUserPermissionCacheItem()
        {
            RoleIds = new List<int>();
            GrantedPermissions = new HashSet<string>();
            ProhibitedPermissions = new HashSet<string>();
        }

        public JTUserPermissionCacheItem(long userId)
            : this()
        {
            UserId = userId;
        }
    }
}
