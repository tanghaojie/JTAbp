using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using JT.Abp.Authorization.Roles;
using JT.Abp.Configuration;
using JT.Authorization.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JT.Authorization.Roles
{
    public class RoleManager : JTRoleManager<Role, User>
    {
        public RoleManager(
            RoleStore store,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<JTRoleManager<Role, User>> logger,
            IPermissionManager permissionManager,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRoleManagementConfig roleManagementConfig
            //IRepository<OrganizationUnit, long> organizationUnitRepository,
            //IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository
            )
            : base(
                  store,
                  roleValidators,
                  keyNormalizer,
                  errors, logger,
                  permissionManager,
                  cacheManager,
                  unitOfWorkManager,
                  roleManagementConfig
                  //organizationUnitRepository,
                  //organizationUnitRoleRepository
                  )
        {
        }
    }
}
