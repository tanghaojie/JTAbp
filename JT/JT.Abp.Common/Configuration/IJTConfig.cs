using JT.Abp.Configuration;

namespace JT.Abp.Configuration
{
    public interface IJTConfig
    {
        IRoleManagementConfig RoleManagement { get; }

        IUserManagementConfig UserManagement { get; }

        IJTEntityTypes EntityTypes { get; }
    }
}
