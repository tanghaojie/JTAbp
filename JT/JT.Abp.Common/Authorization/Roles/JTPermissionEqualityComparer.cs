using Abp.Authorization;
using System.Collections.Generic;

namespace JT.Abp.Authorization.Roles
{
    public class JTPermissionEqualityComparer : IEqualityComparer<Permission>
    {
        public static JTPermissionEqualityComparer Instance { get; } = new JTPermissionEqualityComparer();

        public bool Equals(Permission x, Permission y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }

            return Equals(x.Name, y.Name);
        }

        public int GetHashCode(Permission permission)
        {
            return permission.Name.GetHashCode();
        }
    }
}