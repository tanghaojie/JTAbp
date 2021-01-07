using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using JT.Abp.Authorization.Users;
using JT.Authorization.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.Authorization.Users
{
    public class UserStore : JTUserStore<Role, User>
    {
        public UserStore(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<User, long> userRepository,
            IRepository<Role> roleRepository,
            IRepository<JTUserRole, long> userRoleRepository,
            IRepository<JTUserLogin, long> userLoginRepository,
            IRepository<JTUserClaim, long> userClaimRepository,
            IRepository<JTUserPermission, long> userPermissionSettingRepository

            //IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            //IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository
            )
            : base(unitOfWorkManager,
                  userRepository,
                  roleRepository,
                  userRoleRepository,
                  userLoginRepository,
                  userClaimRepository,
                  userPermissionSettingRepository
                  //userOrganizationUnitRepository,
                  //organizationUnitRoleRepository
                  )
        {
        }
    }
}
