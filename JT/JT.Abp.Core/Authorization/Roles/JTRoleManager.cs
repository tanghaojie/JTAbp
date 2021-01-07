using Abp;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.UI;
using JT.Abp.Authorization.Users;
using JT.Abp.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JT.Abp.Runtime;
using System.Globalization;
using Abp.Collections.Extensions;

namespace JT.Abp.Authorization.Roles
{
    public class JTRoleManager<TRole, TUser> : RoleManager<TRole>, IDomainService
         where TRole : JTRole<TUser>, new()
         where TUser : JTUser<TUser>
    {
        public ILocalizationManager LocalizationManager { get; set; }

        protected string LocalizationSourceName { get; set; }

        public IAbpSession AbpSession { get; set; }

        public IRoleManagementConfig RoleManagementConfig { get; }

        public FeatureDependencyContext FeatureDependencyContext { get; set; }

        private IRolePermissionStore<TRole> RolePermissionStore {
            get {
                if (!(Store is IRolePermissionStore<TRole>))
                {
                    throw new AbpException("Store is not IRolePermissionStore");
                }

                return Store as IRolePermissionStore<TRole>;
            }
        }

        protected JTRoleStore<TRole, TUser> AbpStore { get; }

        private readonly IPermissionManager _permissionManager;
        private readonly ICacheManager _cacheManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        //private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        //private readonly IRepository<OrganizationUnitRole, long> _organizationUnitRoleRepository;

        public JTRoleManager(
            JTRoleStore<TRole, TUser> store,
            IEnumerable<IRoleValidator<TRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<JTRoleManager<TRole, TUser>> logger,
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
                  errors,
                  logger)
        {
            _permissionManager = permissionManager;
            _cacheManager = cacheManager;
            _unitOfWorkManager = unitOfWorkManager;

            RoleManagementConfig = roleManagementConfig;
            //_organizationUnitRepository = organizationUnitRepository;
            //_organizationUnitRoleRepository = organizationUnitRoleRepository;
            AbpStore = store;
            AbpSession = NullAbpSession.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
            //LocalizationSourceName = AbpZeroConsts.LocalizationSourceName;
        }

        public virtual async Task<bool> IsGrantedAsync(string roleName, string permissionName)
        {
            return await IsGrantedAsync((await GetRoleByNameAsync(roleName)).Id, _permissionManager.GetPermission(permissionName));
        }

        public virtual async Task<bool> IsGrantedAsync(int roleId, string permissionName)
        {
            return await IsGrantedAsync(roleId, _permissionManager.GetPermission(permissionName));
        }

        public Task<bool> IsGrantedAsync(TRole role, Permission permission)
        {
            return IsGrantedAsync(role.Id, permission);
        }

        public virtual async Task<bool> IsGrantedAsync(int roleId, Permission permission)
        {
            //Get cached role permissions
            var cacheItem = await GetRolePermissionCacheItemAsync(roleId);

            //Check the permission
            return cacheItem.GrantedPermissions.Contains(permission.Name);
        }

        public virtual bool IsGranted(int roleId, Permission permission)
        {
            //Get cached role permissions
            var cacheItem = GetRolePermissionCacheItem(roleId);

            //Check the permission
            return cacheItem.GrantedPermissions.Contains(permission.Name);
        }

        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(int roleId)
        {
            return await GetGrantedPermissionsAsync(await GetRoleByIdAsync(roleId));
        }

        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(string roleName)
        {
            return await GetGrantedPermissionsAsync(await GetRoleByNameAsync(roleName));
        }

        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(TRole role)
        {
            var cacheItem = await GetRolePermissionCacheItemAsync(role.Id);
            var allPermissions = _permissionManager.GetAllPermissions();
            return allPermissions.Where(x => cacheItem.GrantedPermissions.Contains(x.Name)).ToList();
        }

        public virtual async Task SetGrantedPermissionsAsync(int roleId, IEnumerable<Permission> permissions)
        {
            await SetGrantedPermissionsAsync(await GetRoleByIdAsync(roleId), permissions);
        }

        public virtual async Task SetGrantedPermissionsAsync(TRole role, IEnumerable<Permission> permissions)
        {
            var oldPermissions = await GetGrantedPermissionsAsync(role);
            var newPermissions = permissions.ToArray();

            foreach (var permission in oldPermissions.Where(p => !newPermissions.Contains(p, JTPermissionEqualityComparer.Instance)))
            {
                await ProhibitPermissionAsync(role, permission);
            }

            foreach (var permission in newPermissions.Where(p => !oldPermissions.Contains(p, JTPermissionEqualityComparer.Instance)))
            {
                await GrantPermissionAsync(role, permission);
            }
        }

        public async Task GrantPermissionAsync(TRole role, Permission permission)
        {
            if (await IsGrantedAsync(role.Id, permission))
            {
                return;
            }

            await RolePermissionStore.RemovePermissionAsync(role, new PermissionGrantInfo(permission.Name, false));
            await RolePermissionStore.AddPermissionAsync(role, new PermissionGrantInfo(permission.Name, true));
        }

        public async Task ProhibitPermissionAsync(TRole role, Permission permission)
        {
            if (!await IsGrantedAsync(role.Id, permission))
            {
                return;
            }

            await RolePermissionStore.RemovePermissionAsync(role, new PermissionGrantInfo(permission.Name, true));
            await RolePermissionStore.AddPermissionAsync(role, new PermissionGrantInfo(permission.Name, false));
        }

        public async Task ProhibitAllPermissionsAsync(TRole role)
        {
            foreach (var permission in _permissionManager.GetAllPermissions())
            {
                await ProhibitPermissionAsync(role, permission);
            }
        }

        public async Task ResetAllPermissionsAsync(TRole role)
        {
            await RolePermissionStore.RemoveAllPermissionSettingsAsync(role);
        }

        public override async Task<IdentityResult> CreateAsync(TRole role)
        {
            var result = await CheckDuplicateRoleNameAsync(role.Id, role.Name, role.DisplayName);
            if (!result.Succeeded)
            {
                return result;
            }

            var tenantId = GetCurrentTenantId();
            if (tenantId.HasValue && !role.TenantId.HasValue)
            {
                role.TenantId = tenantId.Value;
            }

            return await base.CreateAsync(role);
        }

        public override async Task<IdentityResult> UpdateAsync(TRole role)
        {
            var result = await CheckDuplicateRoleNameAsync(role.Id, role.Name, role.DisplayName);
            if (!result.Succeeded)
            {
                return result;
            }

            return await base.UpdateAsync(role);
        }

        public override async Task<IdentityResult> DeleteAsync(TRole role)
        {
            if (role.IsStatic)
            {
                throw new UserFriendlyException(string.Format(L("CanNotDeleteStaticRole"), role.Name));
            }

            return await base.DeleteAsync(role);
        }

        public virtual async Task<TRole> GetRoleByIdAsync(int roleId)
        {
            var role = await FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                throw new AbpException("There is no role with id: " + roleId);
            }

            return role;
        }

        public virtual async Task<TRole> GetRoleByNameAsync(string roleName)
        {
            var role = await FindByNameAsync(roleName);
            if (role == null)
            {
                throw new AbpException("There is no role with name: " + roleName);
            }

            return role;
        }

        public virtual TRole GetRoleByName(string roleName)
        {
            var normalizedRoleName = roleName.ToUpperInvariant();

            var role = AbpStore.FindByName(normalizedRoleName);
            if (role == null)
            {
                throw new AbpException("There is no role with name: " + roleName);
            }

            return role;
        }

        public async Task GrantAllPermissionsAsync(TRole role)
        {
            FeatureDependencyContext.TenantId = role.TenantId;

            var permissions = _permissionManager.GetAllPermissions(role.GetMultiTenancySide())
                                                .Where(permission =>
                                                    permission.FeatureDependency == null ||
                                                    permission.FeatureDependency.IsSatisfied(FeatureDependencyContext)
                                                );

            await SetGrantedPermissionsAsync(role, permissions);
        }

        [UnitOfWork]
        public virtual async Task<IdentityResult> CreateStaticRoles(int tenantId)
        {
            var staticRoleDefinitions = RoleManagementConfig.StaticRoles.Where(sr => sr.Side == MultiTenancySides.Tenant);

            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                foreach (var staticRoleDefinition in staticRoleDefinitions)
                {
                    var role = new TRole
                    {
                        TenantId = tenantId,
                        Name = staticRoleDefinition.RoleName,
                        DisplayName = staticRoleDefinition.RoleDisplayName,
                        IsStatic = true
                    };

                    var identityResult = await CreateAsync(role);
                    if (!identityResult.Succeeded)
                    {
                        return identityResult;
                    }
                }
            }

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> CheckDuplicateRoleNameAsync(int? expectedRoleId, string name, string displayName)
        {
            var role = await FindByNameAsync(name);
            if (role != null && role.Id != expectedRoleId)
            {
                throw new UserFriendlyException(string.Format(L("RoleNameIsAlreadyTaken"), name));
            }

            role = await FindByDisplayNameAsync(displayName);
            if (role != null && role.Id != expectedRoleId)
            {
                throw new UserFriendlyException(string.Format(L("RoleDisplayNameIsAlreadyTaken"), displayName));
            }

            return IdentityResult.Success;
        }

        private Task<TRole> FindByDisplayNameAsync(string displayName)
        {
            return AbpStore.FindByDisplayNameAsync(displayName);
        }

        private async Task<JTRolePermissionCacheItem> GetRolePermissionCacheItemAsync(int roleId)
        {
            var cacheKey = roleId + "@" + (GetCurrentTenantId() ?? 0);
            return await _cacheManager.GetRolePermissionCache().GetAsync(cacheKey, async () =>
            {
                var newCacheItem = new JTRolePermissionCacheItem(roleId);

                var role = await Store.FindByIdAsync(roleId.ToString(), CancellationToken);
                if (role == null)
                {
                    throw new AbpException("There is no role with given id: " + roleId);
                }

                var staticRoleDefinition = RoleManagementConfig.StaticRoles.FirstOrDefault(r =>
                    r.RoleName == role.Name && r.Side == role.GetMultiTenancySide());

                if (staticRoleDefinition != null)
                {
                    foreach (var permission in _permissionManager.GetAllPermissions())
                    {
                        if (staticRoleDefinition.IsGrantedByDefault(permission))
                        {
                            newCacheItem.GrantedPermissions.Add(permission.Name);
                        }
                    }
                }

                foreach (var permissionInfo in await RolePermissionStore.GetPermissionsAsync(roleId))
                {
                    if (permissionInfo.IsGranted)
                    {
                        newCacheItem.GrantedPermissions.AddIfNotContains(permissionInfo.Name);
                    }
                    else
                    {
                        newCacheItem.GrantedPermissions.Remove(permissionInfo.Name);
                    }
                }

                return newCacheItem;
            });
        }

        private JTRolePermissionCacheItem GetRolePermissionCacheItem(int roleId)
        {
            var cacheKey = roleId + "@" + (GetCurrentTenantId() ?? 0);
            return _cacheManager.GetRolePermissionCache().Get(cacheKey, () =>
            {
                var newCacheItem = new JTRolePermissionCacheItem(roleId);

                var role = AbpStore.FindById(roleId.ToString(), CancellationToken);
                if (role == null)
                {
                    throw new AbpException("There is no role with given id: " + roleId);
                }

                var staticRoleDefinition = RoleManagementConfig.StaticRoles.FirstOrDefault(r =>
                    r.RoleName == role.Name && r.Side == role.GetMultiTenancySide());

                if (staticRoleDefinition != null)
                {
                    foreach (var permission in _permissionManager.GetAllPermissions())
                    {
                        if (staticRoleDefinition.IsGrantedByDefault(permission))
                        {
                            newCacheItem.GrantedPermissions.Add(permission.Name);
                        }
                    }
                }

                foreach (var permissionInfo in RolePermissionStore.GetPermissions(roleId))
                {
                    if (permissionInfo.IsGranted)
                    {
                        newCacheItem.GrantedPermissions.AddIfNotContains(permissionInfo.Name);
                    }
                    else
                    {
                        newCacheItem.GrantedPermissions.Remove(permissionInfo.Name);
                    }
                }

                return newCacheItem;
            });
        }

        protected virtual string L(string name)
        {
            return LocalizationManager.GetString(LocalizationSourceName, name);
        }

        protected virtual string L(string name, CultureInfo cultureInfo)
        {
            return LocalizationManager.GetString(LocalizationSourceName, name, cultureInfo);
        }

        private int? GetCurrentTenantId()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _unitOfWorkManager.Current.GetTenantId();
            }

            return AbpSession.TenantId;
        }

        /// <summary>
        /// Gets roles of a given organizationUnit
        /// </summary>
        /// <param name="organizationUnit">OrganizationUnit to get belonging roles </param>
        /// <param name="includeChildren">Includes roles for children organization units to result when true. Default is false</param>
        /// <returns></returns>
        //[UnitOfWork]
        //public virtual Task<List<TRole>> GetRolesInOrganizationUnit(OrganizationUnit organizationUnit, bool includeChildren = false)
        //{
        //    if (!includeChildren)
        //    {
        //        var query = from organizationUnitRole in _organizationUnitRoleRepository.GetAll()
        //                    join role in Roles on organizationUnitRole.RoleId equals role.Id
        //                    where organizationUnitRole.OrganizationUnitId == organizationUnit.Id
        //                    select role;

        //        return Task.FromResult(query.ToList());
        //    }
        //    else
        //    {
        //        var query = from organizationUnitRole in _organizationUnitRoleRepository.GetAll()
        //                    join role in Roles on organizationUnitRole.RoleId equals role.Id
        //                    join ou in _organizationUnitRepository.GetAll() on organizationUnitRole.OrganizationUnitId equals ou.Id
        //                    where ou.Code.StartsWith(organizationUnit.Code)
        //                    select role;

        //        return Task.FromResult(query.ToList());
        //    }
        //}

        //public virtual async Task SetOrganizationUnitsAsync(int roleId, params long[] organizationUnitIds)
        //{
        //    await SetOrganizationUnitsAsync(
        //        await GetRoleByIdAsync(roleId),
        //        organizationUnitIds
        //    );
        //}

        //public virtual async Task SetOrganizationUnitsAsync(TRole role, params long[] organizationUnitIds)
        //{
        //    if (organizationUnitIds == null)
        //    {
        //        organizationUnitIds = new long[0];
        //    }

        //    var currentOus = await GetOrganizationUnitsAsync(role);

        //    //Remove from removed OUs
        //    foreach (var currentOu in currentOus)
        //    {
        //        if (!organizationUnitIds.Contains(currentOu.Id))
        //        {
        //            await RemoveFromOrganizationUnitAsync(role, currentOu);
        //        }
        //    }

        //    //Add to added OUs
        //    foreach (var organizationUnitId in organizationUnitIds)
        //    {
        //        if (currentOus.All(ou => ou.Id != organizationUnitId))
        //        {
        //            await AddToOrganizationUnitAsync(
        //                role,
        //                await _organizationUnitRepository.GetAsync(organizationUnitId)
        //            );
        //        }
        //    }
        //}

        //public virtual async Task<bool> IsInOrganizationUnitAsync(int roleId, long ouId)
        //{
        //    return await IsInOrganizationUnitAsync(
        //        await GetRoleByIdAsync(roleId),
        //        await _organizationUnitRepository.GetAsync(ouId)
        //    );
        //}

        //public virtual async Task<bool> IsInOrganizationUnitAsync(TRole role, OrganizationUnit ou)
        //{
        //    return await _organizationUnitRoleRepository.CountAsync(uou =>
        //               uou.RoleId == role.Id && uou.OrganizationUnitId == ou.Id
        //           ) > 0;
        //}

        //public virtual async Task AddToOrganizationUnitAsync(int roleId, long ouId, int? tenantId)
        //{
        //    await AddToOrganizationUnitAsync(
        //        await GetRoleByIdAsync(roleId),
        //        await _organizationUnitRepository.GetAsync(ouId)
        //    );
        //}

        //public virtual async Task AddToOrganizationUnitAsync(TRole role, OrganizationUnit ou)
        //{
        //    if (await IsInOrganizationUnitAsync(role, ou))
        //    {
        //        return;
        //    }

        //    await _organizationUnitRoleRepository.InsertAsync(new OrganizationUnitRole(role.TenantId, role.Id, ou.Id));
        //}

        //public async Task RemoveFromOrganizationUnitAsync(int roleId, long organizationUnitId)
        //{
        //    await RemoveFromOrganizationUnitAsync(
        //        await GetRoleByIdAsync(roleId),
        //        await _organizationUnitRepository.GetAsync(organizationUnitId)
        //    );
        //}

        //public virtual async Task RemoveFromOrganizationUnitAsync(TRole role, OrganizationUnit ou)
        //{
        //    await _organizationUnitRoleRepository.DeleteAsync(uor => uor.RoleId == role.Id && uor.OrganizationUnitId == ou.Id);
        //}

        //[UnitOfWork]
        //public virtual Task<List<OrganizationUnit>> GetOrganizationUnitsAsync(TRole role)
        //{
        //    var query = from uor in _organizationUnitRoleRepository.GetAll()
        //                join ou in _organizationUnitRepository.GetAll() on uor.OrganizationUnitId equals ou.Id
        //                where uor.RoleId == role.Id
        //                select ou;

        //    return Task.FromResult(query.ToList());
        //}

    }
}
