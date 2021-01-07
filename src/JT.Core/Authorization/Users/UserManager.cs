using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using JT.Abp.Authorization.Users;
using JT.Authorization.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace JT.Authorization.Users
{
    public class UserManager : JTUserManager<Role, User>
    {
        public UserManager(
            RoleManager roleManager,
            UserStore store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger,
            IPermissionManager permissionManager,
            IUnitOfWorkManager unitOfWorkManager,
            ICacheManager cacheManager,
            //IRepository<OrganizationUnit, long> organizationUnitRepository,
            //IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            //IOrganizationUnitSettings organizationUnitSettings,
            ISettingManager settingManager)
            : base(
                roleManager,
                store,
                optionsAccessor,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger,
                permissionManager,
                unitOfWorkManager,
                cacheManager,
                //organizationUnitRepository,
                //userOrganizationUnitRepository,
                //organizationUnitSettings,
                settingManager)
        {
        }
    }
}
