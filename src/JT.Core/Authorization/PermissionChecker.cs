using JT.Abp.Authorization;
using JT.Authorization.Roles;
using JT.Authorization.Users;

namespace JT.Authorization
{
    public class PermissionChecker : JTPermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
