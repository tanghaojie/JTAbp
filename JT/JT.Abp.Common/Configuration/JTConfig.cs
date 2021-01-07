using JT.Abp.Configuration;

namespace JT.Abp.Configuration
{
    internal class JTConfig : IJTConfig
    {
        public IRoleManagementConfig RoleManagement {
            get { return _roleManagementConfig; }
        }
        private readonly IRoleManagementConfig _roleManagementConfig;

        public IUserManagementConfig UserManagement {
            get { return _userManagementConfig; }
        }
        private readonly IUserManagementConfig _userManagementConfig;

        public IJTEntityTypes EntityTypes {
            get { return _entityTypes; }
        }
        private readonly IJTEntityTypes _entityTypes;

        public JTConfig(
            IRoleManagementConfig roleManagementConfig,
            IUserManagementConfig userManagementConfig,
            IJTEntityTypes entityTypes)
        {
            _entityTypes = entityTypes;
            _roleManagementConfig = roleManagementConfig;
            _userManagementConfig = userManagementConfig;
        }
    }
}
