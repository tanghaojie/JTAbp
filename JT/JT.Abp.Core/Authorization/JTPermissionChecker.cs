using Abp;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using JT.Abp.Authorization.Roles;
using JT.Abp.Authorization.Users;
using System.Threading.Tasks;

namespace JT.Abp.Authorization
{
    public class JTPermissionChecker<TRole, TUser> : IPermissionChecker, ITransientDependency, IIocManagerAccessor
        where TRole : JTRole<TUser>, new()
        where TUser : JTUser<TUser>
    {
        private readonly JTUserManager<TRole, TUser> _userManager;

        public IIocManager IocManager { get; set; }

        public ILogger Logger { get; set; }

        public IAbpSession AbpSession { get; set; }

        public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }

        public JTPermissionChecker(JTUserManager<TRole, TUser> userManager)
        {
            _userManager = userManager;

            Logger = NullLogger.Instance;
            AbpSession = NullAbpSession.Instance;
        }

        public virtual async Task<bool> IsGrantedAsync(string permissionName)
        {
            return AbpSession.UserId.HasValue && await IsGrantedAsync(AbpSession.UserId.Value, permissionName);
        }

        public virtual bool IsGranted(string permissionName)
        {
            return AbpSession.UserId.HasValue && IsGranted(AbpSession.UserId.Value, permissionName);
        }

        public virtual async Task<bool> IsGrantedAsync(long userId, string permissionName)
        {
            return await _userManager.IsGrantedAsync(userId, permissionName);
        }

        public virtual bool IsGranted(long userId, string permissionName)
        {
            return _userManager.IsGranted(userId, permissionName);
        }

        [UnitOfWork]
        public virtual async Task<bool> IsGrantedAsync(UserIdentifier user, string permissionName)
        {
            if (CurrentUnitOfWorkProvider?.Current == null)
            {
                return await IsGrantedAsync(user.UserId, permissionName);
            }

            using (CurrentUnitOfWorkProvider.Current.SetTenantId(user.TenantId))
            {
                return await IsGrantedAsync(user.UserId, permissionName);
            }
        }

        [UnitOfWork]
        public virtual bool IsGranted(UserIdentifier user, string permissionName)
        {
            if (CurrentUnitOfWorkProvider?.Current == null)
            {
                return IsGranted(user.UserId, permissionName);
            }

            using (CurrentUnitOfWorkProvider.Current.SetTenantId(user.TenantId))
            {
                return IsGranted(user.UserId, permissionName);
            }
        }
    }
}
