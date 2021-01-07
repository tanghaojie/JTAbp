using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Castle.Core.Logging;
using JetBrains.Annotations;
using JT.Abp.Authorization.Users;
using Microsoft.AspNetCore.Identity;

namespace JT.Abp.Authorization.Roles
{
    /// <summary>
    /// Creates a new instance of a persistence store for roles.
    /// </summary>
    public class JTRoleStore<TRole, TUser> :
        IRoleStore<TRole>,
        IRoleClaimStore<TRole>,
        IRolePermissionStore<TRole>,
        IQueryableRoleStore<TRole>,
        ITransientDependency

        where TRole : JTRole<TUser>
        where TUser : JTUser<TUser>
    {
        public ILogger Logger { get; set; }

        public IdentityErrorDescriber ErrorDescriber { get; set; }

        public bool AutoSaveChanges { get; set; } = true;

        public IQueryable<TRole> Roles => _roleRepository.GetAll();

        private readonly IRepository<TRole> _roleRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<JTRolePermission, long> _rolePermissionRepository;

        public JTRoleStore(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<TRole> roleRepository,
            IRepository<JTRolePermission, long> rolePermissionRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _roleRepository = roleRepository;
            _rolePermissionRepository = rolePermissionRepository;

            ErrorDescriber = new IdentityErrorDescriber();
            Logger = NullLogger.Instance;
        }

        protected Task SaveChanges(CancellationToken cancellationToken)
        {
            if (!AutoSaveChanges || _unitOfWorkManager.Current == null)
            {
                return Task.CompletedTask;
            }

            return _unitOfWorkManager.Current.SaveChangesAsync();
        }

        #region IRoleStore

        public virtual async Task<IdentityResult> CreateAsync([NotNull] TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            await _roleRepository.InsertAsync(role);
            await SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync([NotNull] TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            await _roleRepository.UpdateAsync(role);

            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (AbpDbConcurrencyException ex)
            {
                Logger.Warn(ex.ToString(), ex);
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            await SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync([NotNull] TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            await _roleRepository.DeleteAsync(role);

            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (AbpDbConcurrencyException ex)
            {
                Logger.Warn(ex.ToString(), ex);
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            await SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        public Task<string> GetRoleIdAsync([NotNull] TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync([NotNull] TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync([NotNull] TRole role, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            role.Name = roleName;
            return Task.CompletedTask;
        }

        public virtual Task<TRole> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _roleRepository.FirstOrDefaultAsync(id.To<int>());
        }

        public virtual TRole FindById(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _roleRepository.FirstOrDefault(id.To<int>());
        }

        public virtual Task<TRole> FindByNameAsync([NotNull] string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(normalizedName, nameof(normalizedName));

            return _roleRepository.FirstOrDefaultAsync(r => r.NormalizedName == normalizedName);
        }

        public virtual TRole FindByName([NotNull] string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(normalizedName, nameof(normalizedName));

            return _roleRepository.FirstOrDefault(r => r.NormalizedName == normalizedName);
        }

        public virtual Task<string> GetNormalizedRoleNameAsync([NotNull] TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            return Task.FromResult(role.NormalizedName);
        }

        public virtual Task SetNormalizedRoleNameAsync([NotNull] TRole role, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            role.NormalizedName = normalizedName;

            return Task.CompletedTask;
        }

        #endregion 


        #region IRoleClaimStore

        public virtual async Task<IList<Claim>> GetClaimsAsync([NotNull] TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            await _roleRepository.EnsureCollectionLoadedAsync(role, u => u.Claims, cancellationToken);

            return role.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        public async Task AddClaimAsync([NotNull] TRole role, [NotNull] Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));
            Check.NotNull(claim, nameof(claim));

            await _roleRepository.EnsureCollectionLoadedAsync(role, u => u.Claims, cancellationToken);

            role.Claims.Add(new JTRoleClaim(role, claim));
        }

        public async Task RemoveClaimAsync([NotNull] TRole role, [NotNull] Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(role, nameof(role));
            Check.NotNull(claim, nameof(claim));

            await _roleRepository.EnsureCollectionLoadedAsync(role, u => u.Claims, cancellationToken);

            role.Claims.RemoveAll(c => c.ClaimValue == claim.Value && c.ClaimType == claim.Type);
        }

        #endregion


        #region IRolePermissionStore

        public virtual async Task AddPermissionAsync(TRole role, PermissionGrantInfo permissionGrant)
        {
            if (await HasPermissionAsync(role.Id, permissionGrant))
            {
                return;
            }

            await _rolePermissionRepository.InsertAsync(
                new JTRolePermission
                {
                    TenantId = role.TenantId,
                    RoleId = role.Id,
                    Name = permissionGrant.Name,
                    IsGranted = permissionGrant.IsGranted
                });
        }

        public virtual async Task RemovePermissionAsync(TRole role, PermissionGrantInfo permissionGrant)
        {
            await _rolePermissionRepository.DeleteAsync(
                permissionSetting => permissionSetting.RoleId == role.Id &&
                                     permissionSetting.Name == permissionGrant.Name &&
                                     permissionSetting.IsGranted == permissionGrant.IsGranted
                );
        }

        public virtual Task<IList<PermissionGrantInfo>> GetPermissionsAsync(TRole role)
        {
            return GetPermissionsAsync(role.Id);
        }

        public virtual IList<PermissionGrantInfo> GetPermissions(TRole role)
        {
            return GetPermissions(role.Id);
        }

        public async Task<IList<PermissionGrantInfo>> GetPermissionsAsync(int roleId)
        {
            return (await _rolePermissionRepository.GetAllListAsync(p => p.RoleId == roleId))
                .Select(p => new PermissionGrantInfo(p.Name, p.IsGranted))
                .ToList();
        }

        public IList<PermissionGrantInfo> GetPermissions(int roleId)
        {
            return (_rolePermissionRepository.GetAllList(p => p.RoleId == roleId))
                .Select(p => new PermissionGrantInfo(p.Name, p.IsGranted))
                .ToList();
        }

        public virtual async Task<bool> HasPermissionAsync(int roleId, PermissionGrantInfo permissionGrant)
        {
            return await _rolePermissionRepository.FirstOrDefaultAsync(
                p => p.RoleId == roleId &&
                     p.Name == permissionGrant.Name &&
                     p.IsGranted == permissionGrant.IsGranted
                ) != null;
        }

        public virtual async Task RemoveAllPermissionSettingsAsync(TRole role)
        {
            await _rolePermissionRepository.DeleteAsync(s => s.RoleId == role.Id);
        }

        #endregion

        public virtual async Task<TRole> FindByDisplayNameAsync(string displayName)
        {
            return await _roleRepository.FirstOrDefaultAsync(
                role => role.DisplayName == displayName
                );
        }

        public void Dispose()
        {
            //No need to dispose since using IOC.
        }
    }
}
