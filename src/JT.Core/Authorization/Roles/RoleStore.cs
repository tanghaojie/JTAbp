using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using JT.Abp.Authorization.Roles;
using JT.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.Authorization.Roles
{
    public class RoleStore : JTRoleStore<Role, User>
    {
        public RoleStore(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Role> roleRepository,
            IRepository<JTRolePermission, long> rolePermissionSettingRepository
            )
            : base(
                unitOfWorkManager,
                roleRepository,
                rolePermissionSettingRepository)
        {
        }
    }
}
