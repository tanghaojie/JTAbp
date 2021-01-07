using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.Abp.Authorization.Roles
{
    /// <summary>
    /// Used to perform permission database operations for a role.
    /// </summary>
    public interface IRolePermissionStore<in TRole>
        where TRole : JTRoleBase
    {
        Task AddPermissionAsync(TRole role, PermissionGrantInfo permissionGrant);

        Task RemovePermissionAsync(TRole role, PermissionGrantInfo permissionGrant);

        Task<IList<PermissionGrantInfo>> GetPermissionsAsync(TRole role);

        IList<PermissionGrantInfo> GetPermissions(TRole role);

        Task<IList<PermissionGrantInfo>> GetPermissionsAsync(int roleId);

        IList<PermissionGrantInfo> GetPermissions(int roleId);

        Task<bool> HasPermissionAsync(int roleId, PermissionGrantInfo permissionGrant);

        Task RemoveAllPermissionSettingsAsync(TRole role);
    }
}
