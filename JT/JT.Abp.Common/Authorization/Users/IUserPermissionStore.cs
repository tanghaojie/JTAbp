using System.Collections.Generic;
using System.Threading.Tasks;

namespace JT.Abp.Authorization.Users
{
    public interface IUserPermissionStore<in TUser>
        where TUser : JTUserBase
    {
        Task AddPermissionAsync(TUser user, PermissionGrantInfo permissionGrant);

        void AddPermission(TUser user, PermissionGrantInfo permissionGrant);

        Task RemovePermissionAsync(TUser user, PermissionGrantInfo permissionGrant);

        void RemovePermission(TUser user, PermissionGrantInfo permissionGrant);

        Task<IList<PermissionGrantInfo>> GetPermissionsAsync(long userId);

        IList<PermissionGrantInfo> GetPermissions(long userId);

        Task<bool> HasPermissionAsync(long userId, PermissionGrantInfo permissionGrant);

        bool HasPermission(long userId, PermissionGrantInfo permissionGrant);

        Task RemoveAllPermissionSettingsAsync(TUser user);

        void RemoveAllPermissionSettings(TUser user);
    }
}
