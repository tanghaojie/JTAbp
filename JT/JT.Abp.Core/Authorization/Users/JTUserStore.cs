using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using JetBrains.Annotations;
using JT.Abp.Authorization.Roles;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace JT.Abp.Authorization.Users
{
    public class JTUserStore<TRole, TUser> :
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,
        IUserPermissionStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        ITransientDependency

        where TRole : JTRole<TUser>
        where TUser : JTUser<TUser>
    {
        public ILogger Logger { get; set; }

        public IdentityErrorDescriber ErrorDescriber { get; set; }

        public bool AutoSaveChanges { get; set; } = true;

        public IAbpSession AbpSession { get; set; }

        public IQueryable<TUser> Users => UserRepository.GetAll();

        public IRepository<TUser, long> UserRepository { get; }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        private readonly IRepository<TRole> _roleRepository;
        private readonly IRepository<JTUserRole, long> _userRoleRepository;
        private readonly IRepository<JTUserLogin, long> _userLoginRepository;
        private readonly IRepository<JTUserClaim, long> _userClaimRepository;
        private readonly IRepository<JTUserPermission, long> _userPermissionSettingRepository;
        //private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
        //private readonly IRepository<OrganizationUnitRole, long> _organizationUnitRoleRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public JTUserStore(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<TUser, long> userRepository,
            IRepository<TRole> roleRepository,
            IRepository<JTUserRole, long> userRoleRepository,
            IRepository<JTUserLogin, long> userLoginRepository,
            IRepository<JTUserClaim, long> userClaimRepository,
            IRepository<JTUserPermission, long> userPermissionSettingRepository
            //IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            //IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository
            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            UserRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _userLoginRepository = userLoginRepository;
            _userClaimRepository = userClaimRepository;
            _userPermissionSettingRepository = userPermissionSettingRepository;
            //_userOrganizationUnitRepository = userOrganizationUnitRepository;
            //_organizationUnitRoleRepository = organizationUnitRoleRepository;

            AbpSession = NullAbpSession.Instance;
            ErrorDescriber = new IdentityErrorDescriber();
            Logger = NullLogger.Instance;
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        protected Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (!AutoSaveChanges || _unitOfWorkManager.Current == null)
            {
                return Task.CompletedTask;
            }

            return _unitOfWorkManager.Current.SaveChangesAsync();
        }

        protected void SaveChanges(CancellationToken cancellationToken)
        {
            if (!AutoSaveChanges || _unitOfWorkManager.Current == null)
            {
                return;
            }

            _unitOfWorkManager.Current.SaveChanges();
        }

        #region IUserLoginStore --> IUserStore

        public virtual Task<string> GetUserIdAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(GetUserId(user, cancellationToken));
        }

        public virtual string GetUserId([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return user.Id.ToString();
        }

        public virtual Task<string> GetUserNameAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(GetUserName(user, cancellationToken));
        }

        public virtual string GetUserName([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return user.UserName;
        }

        public virtual Task SetUserNameAsync([NotNull] TUser user, string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            SetUserName(user, userName, cancellationToken);
            return Task.CompletedTask;
        }

        public virtual void SetUserName([NotNull] TUser user, string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.UserName = userName;
        }

        public virtual Task<string> GetNormalizedUserNameAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(GetNormalizedUserName(user, cancellationToken));
        }

        public virtual string GetNormalizedUserName([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return user.NormalizedUserName;
        }

        public virtual Task SetNormalizedUserNameAsync([NotNull] TUser user, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            SetNormalizedUserName(user, normalizedName, cancellationToken);
            return Task.CompletedTask;
        }

        public virtual void SetNormalizedUserName([NotNull] TUser user, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.NormalizedUserName = normalizedName;
        }

        public virtual async Task<IdentityResult> CreateAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await UserRepository.InsertAsync(user);
            await SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public virtual IdentityResult Create([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            UserRepository.Insert(user);
            SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            await UserRepository.UpdateAsync(user);

            try
            {
                await SaveChangesAsync(cancellationToken);
            }
            catch (AbpDbConcurrencyException ex)
            {
                Logger.Warn(ex.ToString(), ex);
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            await SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public virtual IdentityResult Update([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            UserRepository.Update(user);

            try
            {
                SaveChanges(cancellationToken);
            }
            catch (AbpDbConcurrencyException ex)
            {
                Logger.Warn(ex.ToString(), ex);
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await UserRepository.DeleteAsync(user);

            try
            {
                await SaveChangesAsync(cancellationToken);
            }
            catch (AbpDbConcurrencyException ex)
            {
                Logger.Warn(ex.ToString(), ex);
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            await SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public virtual IdentityResult Delete([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            UserRepository.Delete(user);

            try
            {
                SaveChanges(cancellationToken);
            }
            catch (AbpDbConcurrencyException ex)
            {
                Logger.Warn(ex.ToString(), ex);
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        public virtual Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            return UserRepository.FirstOrDefaultAsync(userId.To<long>());
        }

        public virtual TUser FindById(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            return UserRepository.FirstOrDefault(userId.To<long>());
        }

        public virtual Task<TUser> FindByNameAsync([NotNull] string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(normalizedUserName, nameof(normalizedUserName));

            return UserRepository.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName);
        }

        public virtual TUser FindByName([NotNull] string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(normalizedUserName, nameof(normalizedUserName));

            return UserRepository.FirstOrDefault(u => u.NormalizedUserName == normalizedUserName);
        }

        #endregion

        #region IUserPasswordStore

        public virtual Task SetPasswordHashAsync([NotNull] TUser user, string passwordHash, CancellationToken cancellationToken = default(CancellationToken))
        {
            SetPasswordHash(user, passwordHash, cancellationToken);
            return Task.CompletedTask;
        }

        public virtual void SetPasswordHash([NotNull] TUser user, string passwordHash, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.Password = passwordHash;
        }

        public virtual Task<string> GetPasswordHashAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Password);
        }

        public virtual string GetPasswordHash([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return user.Password;
        }

        public virtual Task<bool> HasPasswordAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Password != null);
        }

        public virtual bool HasPassword([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return user.Password != null;
        }

        #endregion 

        #region IUserRoleStore

        public virtual async Task AddToRoleAsync([NotNull] TUser user, [NotNull] string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(normalizedRoleName, nameof(normalizedRoleName));

            if (await IsInRoleAsync(user, normalizedRoleName, cancellationToken))
            {
                return;
            }

            var role = await _roleRepository.FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName);

            if (role == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Role {0} does not exist!", normalizedRoleName));
            }

            await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Roles, cancellationToken);
            user.Roles.Add(new JTUserRole(user.TenantId, user.Id, role.Id));
        }

        public virtual void AddToRole([NotNull] TUser user, [NotNull] string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(normalizedRoleName, nameof(normalizedRoleName));

            if (IsInRole(user, normalizedRoleName, cancellationToken))
            {
                return;
            }

            var role = _roleRepository.FirstOrDefault(r => r.NormalizedName == normalizedRoleName);

            if (role == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Role {0} does not exist!", normalizedRoleName));
            }

            UserRepository.EnsureCollectionLoaded(user, u => u.Roles, cancellationToken);
            user.Roles.Add(new JTUserRole(user.TenantId, user.Id, role.Id));
        }

        public virtual async Task RemoveFromRoleAsync([NotNull] TUser user, [NotNull] string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException(nameof(normalizedRoleName) + " can not be null or whitespace");
            }

            if (!await IsInRoleAsync(user, normalizedRoleName, cancellationToken))
            {
                return;
            }

            var role = await _roleRepository.FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName);
            if (role == null)
            {
                return;
            }

            await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Roles, cancellationToken);
            user.Roles.RemoveAll(r => r.RoleId == role.Id);
        }

        public virtual void RemoveFromRole([NotNull] TUser user, [NotNull] string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException(nameof(normalizedRoleName) + " can not be null or whitespace");
            }

            if (!IsInRole(user, normalizedRoleName, cancellationToken))
            {
                return;
            }

            var role = _roleRepository.FirstOrDefault(r => r.NormalizedName == normalizedRoleName);
            if (role == null)
            {
                return;
            }

            user.Roles.RemoveAll(r => r.RoleId == role.Id);
        }

        [UnitOfWork]
        public virtual async Task<IList<string>> GetRolesAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            var userRoles = await AsyncQueryableExecuter.ToListAsync(from userRole in _userRoleRepository.GetAll()
                                                                     join role in _roleRepository.GetAll() on userRole.RoleId equals role.Id
                                                                     where userRole.UserId == user.Id
                                                                     select role.Name);

            //var userOrganizationUnitRoles = await AsyncQueryableExecuter.ToListAsync(
            //    from userOu in _userOrganizationUnitRepository.GetAll()
            //    join roleOu in _organizationUnitRoleRepository.GetAll() on userOu.OrganizationUnitId equals roleOu
            //        .OrganizationUnitId
            //    join userOuRoles in _roleRepository.GetAll() on roleOu.RoleId equals userOuRoles.Id
            //    where userOu.UserId == user.Id
            //    select userOuRoles.Name);

            //return userRoles.Union(userOrganizationUnitRoles).ToList();

            return userRoles.ToList();
        }

        [UnitOfWork]
        public virtual IList<string> GetRoles([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            var userRoles = (
                from userRole in _userRoleRepository.GetAll()
                join role in _roleRepository.GetAll() on userRole.RoleId equals role.Id
                where userRole.UserId == user.Id
                select role.Name
                ).ToList();

            //var userOrganizationUnitRoles = (
            //    from userOu in _userOrganizationUnitRepository.GetAll()
            //    join roleOu in _organizationUnitRoleRepository.GetAll() on userOu.OrganizationUnitId equals roleOu
            //        .OrganizationUnitId
            //    join userOuRoles in _roleRepository.GetAll() on roleOu.RoleId equals userOuRoles.Id
            //    where userOu.UserId == user.Id
            //    select userOuRoles.Name
            //    ).ToList();

            //return userRoles.Union(userOrganizationUnitRoles).ToList();

            return userRoles;
        }

        public virtual async Task<bool> IsInRoleAsync([NotNull] TUser user, [NotNull] string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException(nameof(normalizedRoleName) + " can not be null or whitespace");
            }

            return (await GetRolesAsync(user, cancellationToken)).Any(r => r.ToUpperInvariant() == normalizedRoleName);
        }

        public virtual bool IsInRole([NotNull] TUser user, [NotNull] string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException(nameof(normalizedRoleName) + " can not be null or whitespace");
            }

            return (GetRoles(user, cancellationToken)).Any(r => r.ToUpperInvariant() == normalizedRoleName);
        }

        [UnitOfWork]
        public virtual async Task<IList<TUser>> GetUsersInRoleAsync([NotNull] string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }

            var role = await _roleRepository.FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName);

            if (role == null)
            {
                return new List<TUser>();
            }

            var query = from userrole in _userRoleRepository.GetAll()
                        join user in UserRepository.GetAll() on userrole.UserId equals user.Id
                        where userrole.RoleId.Equals(role.Id)
                        select user;

            return await AsyncQueryableExecuter.ToListAsync(query);
        }

        [UnitOfWork]
        public virtual IList<TUser> GetUsersInRole([NotNull] string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }

            var role = _roleRepository.FirstOrDefault(r => r.NormalizedName == normalizedRoleName);

            if (role == null)
            {
                return new List<TUser>();
            }

            var query = from userrole in _userRoleRepository.GetAll()
                        join user in UserRepository.GetAll() on userrole.UserId equals user.Id
                        where userrole.RoleId.Equals(role.Id)
                        select user;

            return query.ToList();
        }

        #endregion

        #region IUserClaimStore

        public virtual async Task<IList<Claim>> GetClaimsAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Claims, cancellationToken);

            return user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        public virtual IList<Claim> GetClaims([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            UserRepository.EnsureCollectionLoaded(user, u => u.Claims, cancellationToken);

            return user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        public virtual async Task AddClaimsAsync([NotNull] TUser user, [NotNull] IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(claims, nameof(claims));

            await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Claims, cancellationToken);

            foreach (var claim in claims)
            {
                user.Claims.Add(new JTUserClaim(user, claim));
            }
        }

        public virtual void AddClaims([NotNull] TUser user, [NotNull] IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(claims, nameof(claims));

            UserRepository.EnsureCollectionLoaded(user, u => u.Claims, cancellationToken);

            foreach (var claim in claims)
            {
                user.Claims.Add(new JTUserClaim(user, claim));
            }
        }

        public virtual async Task ReplaceClaimAsync([NotNull] TUser user, [NotNull] Claim claim, [NotNull] Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(claim, nameof(claim));
            Check.NotNull(newClaim, nameof(newClaim));

            await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Claims, cancellationToken);

            var userClaims = user.Claims.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type);
            foreach (var userClaim in userClaims)
            {
                userClaim.ClaimType = newClaim.Type;
                userClaim.ClaimValue = newClaim.Value;
            }
        }

        public virtual void ReplaceClaim([NotNull] TUser user, [NotNull] Claim claim, [NotNull] Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(claim, nameof(claim));
            Check.NotNull(newClaim, nameof(newClaim));

            UserRepository.EnsureCollectionLoaded(user, u => u.Claims, cancellationToken);

            var userClaims = user.Claims.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type);
            foreach (var userClaim in userClaims)
            {
                userClaim.ClaimType = claim.Type;
                userClaim.ClaimValue = claim.Value;
            }
        }

        public virtual async Task RemoveClaimsAsync([NotNull] TUser user, [NotNull] IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(claims, nameof(claims));

            await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Claims, cancellationToken);

            foreach (var claim in claims)
            {
                user.Claims.RemoveAll(c => c.ClaimValue == claim.Value && c.ClaimType == claim.Type);
            }
        }

        public virtual void RemoveClaims([NotNull] TUser user, [NotNull] IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(claims, nameof(claims));

            UserRepository.EnsureCollectionLoaded(user, u => u.Claims, cancellationToken);

            foreach (var claim in claims)
            {
                user.Claims.RemoveAll(c => c.ClaimValue == claim.Value && c.ClaimType == claim.Type);
            }
        }

        [UnitOfWork]
        public virtual async Task<IList<TUser>> GetUsersForClaimAsync([NotNull] Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(claim, nameof(claim));

            var query = from userclaims in _userClaimRepository.GetAll()
                        join user in UserRepository.GetAll() on userclaims.UserId equals user.Id
                        where userclaims.ClaimValue == claim.Value && userclaims.ClaimType == claim.Type && userclaims.TenantId == AbpSession.TenantId
                        select user;

            return await AsyncQueryableExecuter.ToListAsync(query);
        }

        [UnitOfWork]
        public virtual IList<TUser> GetUsersForClaim([NotNull] Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(claim, nameof(claim));

            var query = from userclaims in _userClaimRepository.GetAll()
                        join user in UserRepository.GetAll() on userclaims.UserId equals user.Id
                        where userclaims.ClaimValue == claim.Value && userclaims.ClaimType == claim.Type && userclaims.TenantId == AbpSession.TenantId
                        select user;

            return query.ToList();
        }

        #endregion 

        #region IUserLoginStore error

        public virtual async Task AddLoginAsync([NotNull] TUser user, [NotNull] UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(login, nameof(login));

            await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Logins, cancellationToken);

            user.Logins.Add(new JTUserLogin(user.TenantId, user.Id, login.LoginProvider, login.ProviderKey));
        }

        public virtual void AddLogin([NotNull] TUser user, [NotNull] UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(login, nameof(login));

            UserRepository.EnsureCollectionLoaded(user, u => u.Logins, cancellationToken);

            user.Logins.Add(new JTUserLogin(user.TenantId, user.Id, login.LoginProvider, login.ProviderKey));
        }

        public virtual async Task RemoveLoginAsync([NotNull] TUser user, [NotNull] string loginProvider, [NotNull] string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(loginProvider, nameof(loginProvider));
            Check.NotNull(providerKey, nameof(providerKey));

            await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Logins, cancellationToken);

            user.Logins.RemoveAll(userLogin => userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey);
        }

        public virtual void RemoveLogin([NotNull] TUser user, [NotNull] string loginProvider, [NotNull] string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(loginProvider, nameof(loginProvider));
            Check.NotNull(providerKey, nameof(providerKey));

            UserRepository.EnsureCollectionLoaded(user, u => u.Logins, cancellationToken);

            user.Logins.RemoveAll(userLogin => userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey);
        }

        public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Logins, cancellationToken);

            return user.Logins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.LoginProvider)).ToList();
        }

        public virtual IList<UserLoginInfo> GetLogins([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            UserRepository.EnsureCollectionLoaded(user, u => u.Logins, cancellationToken);

            return user.Logins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.LoginProvider)).ToList();
        }

        [UnitOfWork]
        public virtual Task<TUser> FindByLoginAsync([NotNull] string loginProvider, [NotNull] string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(loginProvider, nameof(loginProvider));
            Check.NotNull(providerKey, nameof(providerKey));

            var query = from userLogin in _userLoginRepository.GetAll()
                        join user in UserRepository.GetAll() on userLogin.UserId equals user.Id
                        where userLogin.LoginProvider == loginProvider &&
                              userLogin.ProviderKey == providerKey &&
                              userLogin.TenantId == AbpSession.TenantId
                        select user;

            return AsyncQueryableExecuter.FirstOrDefaultAsync(query);
        }

        [UnitOfWork]
        public virtual TUser FindByLogin([NotNull] string loginProvider, [NotNull] string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(loginProvider, nameof(loginProvider));
            Check.NotNull(providerKey, nameof(providerKey));

            var query = from userLogin in _userLoginRepository.GetAll()
                        join user in UserRepository.GetAll() on userLogin.UserId equals user.Id
                        where userLogin.LoginProvider == loginProvider &&
                              userLogin.ProviderKey == providerKey &&
                              userLogin.TenantId == AbpSession.TenantId
                        select user;

            return query.FirstOrDefault();
        }

        #endregion

        #region IUserEmailStore

        public virtual Task<bool> GetEmailConfirmedAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.IsEmailConfirmed);
        }

        public virtual bool GetEmailConfirmed([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return user.IsEmailConfirmed;
        }

        public virtual Task SetEmailConfirmedAsync([NotNull] TUser user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.IsEmailConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public virtual void SetEmailConfirmed([NotNull] TUser user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.IsEmailConfirmed = confirmed;
        }

        public virtual Task SetEmailAsync([NotNull] TUser user, string email, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.EmailAddress = email;

            return Task.CompletedTask;
        }

        public virtual void SetEmail([NotNull] TUser user, string email, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.EmailAddress = email;
        }

        public virtual Task<string> GetEmailAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.EmailAddress);
        }

        public virtual string GetEmail([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return user.EmailAddress;
        }

        public virtual Task<string> GetNormalizedEmailAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.NormalizedEmailAddress);
        }

        public virtual string GetNormalizedEmail([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return user.NormalizedEmailAddress;
        }

        public virtual Task SetNormalizedEmailAsync([NotNull] TUser user, string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.NormalizedEmailAddress = normalizedEmail;

            return Task.CompletedTask;
        }

        public virtual void SetNormalizedEmail([NotNull] TUser user, string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.NormalizedEmailAddress = normalizedEmail;
        }

        public virtual Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            return UserRepository.FirstOrDefaultAsync(u => u.NormalizedEmailAddress == normalizedEmail);
        }

        public virtual TUser FindByEmail(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            return UserRepository.FirstOrDefault(u => u.NormalizedEmailAddress == normalizedEmail);
        }

        [UnitOfWork]
        public virtual async Task<TUser> FindByNameOrEmailAsync(int? tenantId, string userNameOrEmailAddress)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return await FindByNameOrEmailAsync(userNameOrEmailAddress);
            }
        }

        [UnitOfWork]
        public virtual TUser FindByNameOrEmail(int? tenantId, string userNameOrEmailAddress)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return FindByNameOrEmail(userNameOrEmailAddress);
            }
        }

        #endregion

        #region IUserLockoutStore

        public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            if (!user.LockoutEndDateUtc.HasValue)
            {
                return Task.FromResult<DateTimeOffset?>(null);
            }

            var lockoutEndDate = DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc);

            return Task.FromResult<DateTimeOffset?>(new DateTimeOffset(lockoutEndDate));
        }

        public virtual DateTimeOffset? GetLockoutEndDate([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            if (!user.LockoutEndDateUtc.HasValue)
            {
                return (DateTimeOffset?)null;
            }

            var lockoutEndDate = DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc);

            return new DateTimeOffset(lockoutEndDate);
        }

        public virtual Task SetLockoutEndDateAsync([NotNull] TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.LockoutEndDateUtc = lockoutEnd?.UtcDateTime;

            return Task.CompletedTask;
        }

        public virtual void SetLockoutEndDate([NotNull] TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.LockoutEndDateUtc = lockoutEnd?.UtcDateTime;
        }

        public virtual Task<int> IncrementAccessFailedCountAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.AccessFailedCount++;

            return Task.FromResult(user.AccessFailedCount);
        }

        public virtual int IncrementAccessFailedCount([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.AccessFailedCount++;

            return user.AccessFailedCount;
        }

        public virtual Task ResetAccessFailedCountAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.AccessFailedCount = 0;

            return Task.CompletedTask;
        }

        public virtual void ResetAccessFailedCount([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.AccessFailedCount = 0;
        }

        public virtual Task<int> GetAccessFailedCountAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.AccessFailedCount);
        }

        public virtual int GetAccessFailedCount([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return user.AccessFailedCount;
        }

        public virtual Task<bool> GetLockoutEnabledAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.IsLockoutEnabled);
        }

        public virtual bool GetLockoutEnabled([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return user.IsLockoutEnabled;
        }

        public virtual Task SetLockoutEnabledAsync([NotNull] TUser user, bool enabled, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.IsLockoutEnabled = enabled;

            return Task.CompletedTask;
        }

        public virtual void SetLockoutEnabled([NotNull] TUser user, bool enabled, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.IsLockoutEnabled = enabled;
        }

        #endregion

        #region IUserPhoneNumberStore

        public virtual Task SetPhoneNumberAsync([NotNull] TUser user, string phoneNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.PhoneNumber = phoneNumber;

            return Task.CompletedTask;
        }

        public virtual void SetPhoneNumber([NotNull] TUser user, string phoneNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.PhoneNumber = phoneNumber;
        }

        public virtual Task<string> GetPhoneNumberAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.PhoneNumber);
        }

        public virtual string GetPhoneNumber([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return user.PhoneNumber;
        }

        public virtual Task<bool> GetPhoneNumberConfirmedAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.IsPhoneNumberConfirmed);
        }

        public virtual bool GetPhoneNumberConfirmed([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return user.IsPhoneNumberConfirmed;
        }

        public virtual Task SetPhoneNumberConfirmedAsync([NotNull] TUser user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.IsPhoneNumberConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public virtual void SetPhoneNumberConfirmed([NotNull] TUser user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.IsPhoneNumberConfirmed = confirmed;
        }

        #endregion

        #region IUserSecurityStampStore

        public virtual Task SetSecurityStampAsync([NotNull] TUser user, string stamp, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.SecurityStamp = stamp;

            return Task.CompletedTask;
        }

        public virtual void SetSecurityStamp([NotNull] TUser user, string stamp, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.SecurityStamp = stamp;
        }

        public virtual Task<string> GetSecurityStampAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.SecurityStamp);
        }

        public virtual string GetSecurityStamp([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return user.SecurityStamp;
        }

        #endregion

        #region IUserTwoFactorStore

        public virtual Task SetTwoFactorEnabledAsync([NotNull] TUser user, bool enabled, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.IsTwoFactorEnabled = enabled;

            return Task.CompletedTask;
        }

        public virtual void SetTwoFactorEnabled([NotNull] TUser user, bool enabled, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.IsTwoFactorEnabled = enabled;
        }

        public virtual Task<bool> GetTwoFactorEnabledAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.IsTwoFactorEnabled);
        }

        public virtual bool GetTwoFactorEnabled([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return user.IsTwoFactorEnabled;
        }

        #endregion

        #region IUserAuthenticationTokenStore

        public virtual async Task SetTokenAsync([NotNull] TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);

            var token = user.Tokens.FirstOrDefault(t => t.LoginProvider == loginProvider && t.Name == name);
            if (token == null)
            {
                user.Tokens.Add(new JTUserToken(user, loginProvider, name, value));
            }
            else
            {
                token.Value = value;
            }
        }

        public virtual void SetToken([NotNull] TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            UserRepository.EnsureCollectionLoaded(user, u => u.Tokens, cancellationToken);

            var token = user.Tokens.FirstOrDefault(t => t.LoginProvider == loginProvider && t.Name == name);
            if (token == null)
            {
                user.Tokens.Add(new JTUserToken(user, loginProvider, name, value));
            }
            else
            {
                token.Value = value;
            }
        }

        public async Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);

            user.Tokens.RemoveAll(t => t.LoginProvider == loginProvider && t.Name == name);
        }

        public void RemoveToken(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            UserRepository.EnsureCollectionLoaded(user, u => u.Tokens, cancellationToken);

            user.Tokens.RemoveAll(t => t.LoginProvider == loginProvider && t.Name == name);
        }

        public async Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);

            return user.Tokens.FirstOrDefault(t => t.LoginProvider == loginProvider && t.Name == name)?.Value;
        }

        public string GetToken(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            UserRepository.EnsureCollectionLoaded(user, u => u.Tokens, cancellationToken);

            return user.Tokens.FirstOrDefault(t => t.LoginProvider == loginProvider && t.Name == name)?.Value;
        }

        #endregion

        #region IUserPermissionStore

        public virtual async Task AddPermissionAsync(TUser user, PermissionGrantInfo permissionGrant)
        {
            if (await HasPermissionAsync(user.Id, permissionGrant))
            {
                return;
            }

            await _userPermissionSettingRepository.InsertAsync(
                new JTUserPermission
                {
                    TenantId = user.TenantId,
                    UserId = user.Id,
                    Name = permissionGrant.Name,
                    IsGranted = permissionGrant.IsGranted
                });
        }

        public virtual void AddPermission(TUser user, PermissionGrantInfo permissionGrant)
        {
            if (HasPermission(user.Id, permissionGrant))
            {
                return;
            }

            _userPermissionSettingRepository.Insert(
                new JTUserPermission
                {
                    TenantId = user.TenantId,
                    UserId = user.Id,
                    Name = permissionGrant.Name,
                    IsGranted = permissionGrant.IsGranted
                });
        }

        public virtual async Task RemovePermissionAsync(TUser user, PermissionGrantInfo permissionGrant)
        {
            await _userPermissionSettingRepository.DeleteAsync(
                permissionSetting => permissionSetting.UserId == user.Id &&
                                     permissionSetting.Name == permissionGrant.Name &&
                                     permissionSetting.IsGranted == permissionGrant.IsGranted
            );
        }

        public virtual void RemovePermission(TUser user, PermissionGrantInfo permissionGrant)
        {
            _userPermissionSettingRepository.Delete(
                permissionSetting => permissionSetting.UserId == user.Id &&
                                     permissionSetting.Name == permissionGrant.Name &&
                                     permissionSetting.IsGranted == permissionGrant.IsGranted
            );
        }

        public virtual async Task<IList<PermissionGrantInfo>> GetPermissionsAsync(long userId)
        {
            return (await _userPermissionSettingRepository.GetAllListAsync(p => p.UserId == userId))
                .Select(p => new PermissionGrantInfo(p.Name, p.IsGranted))
                .ToList();
        }

        public virtual IList<PermissionGrantInfo> GetPermissions(long userId)
        {
            return (_userPermissionSettingRepository.GetAllList(p => p.UserId == userId))
                .Select(p => new PermissionGrantInfo(p.Name, p.IsGranted))
                .ToList();
        }

        public virtual async Task<bool> HasPermissionAsync(long userId, PermissionGrantInfo permissionGrant)
        {
            return await _userPermissionSettingRepository.FirstOrDefaultAsync(
                       p => p.UserId == userId &&
                            p.Name == permissionGrant.Name &&
                            p.IsGranted == permissionGrant.IsGranted
                   ) != null;
        }

        public virtual bool HasPermission(long userId, PermissionGrantInfo permissionGrant)
        {
            return _userPermissionSettingRepository.FirstOrDefault(
                       p => p.UserId == userId &&
                            p.Name == permissionGrant.Name &&
                            p.IsGranted == permissionGrant.IsGranted
                   ) != null;
        }

        public virtual async Task RemoveAllPermissionSettingsAsync(TUser user)
        {
            await _userPermissionSettingRepository.DeleteAsync(s => s.UserId == user.Id);
        }

        public virtual void RemoveAllPermissionSettings(TUser user)
        {
            _userPermissionSettingRepository.Delete(s => s.UserId == user.Id);
        }

        #endregion

        #region IUserAuthenticatorKeyStore

        private const string InternalLoginProvider = "[AspNetUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";

        public virtual async Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
        {
            await SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
        }

        public virtual void SetAuthenticatorKey(TUser user, string key, CancellationToken cancellationToken)
        {
            SetToken(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
        }

        public async Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
        {
            return await GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
        }

        public string GetAuthenticatorKey(TUser user, CancellationToken cancellationToken)
        {
            return GetToken(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
        }

        #endregion


        public virtual async Task<TUser> FindByNameOrEmailAsync(string userNameOrEmailAddress)
        {
            var normalizedUserNameOrEmailAddress = NormalizeKey(userNameOrEmailAddress);

            return await UserRepository.FirstOrDefaultAsync(
                user => (user.NormalizedUserName == normalizedUserNameOrEmailAddress || user.NormalizedEmailAddress == normalizedUserNameOrEmailAddress)
            );
        }

        public virtual TUser FindByNameOrEmail(string userNameOrEmailAddress)
        {
            var normalizedUserNameOrEmailAddress = NormalizeKey(userNameOrEmailAddress);

            return UserRepository.FirstOrDefault(
                user => (user.NormalizedUserName == normalizedUserNameOrEmailAddress || user.NormalizedEmailAddress == normalizedUserNameOrEmailAddress)
            );
        }

        public virtual async Task<TUser> FindAsync(UserLoginInfo login)
        {
            var userLogin = await _userLoginRepository.FirstOrDefaultAsync(
                ul => ul.LoginProvider == login.LoginProvider && ul.ProviderKey == login.ProviderKey
            );

            if (userLogin == null)
            {
                return null;
            }

            return await UserRepository.FirstOrDefaultAsync(u => u.Id == userLogin.UserId);
        }

        public virtual TUser Find(UserLoginInfo login)
        {
            var userLogin = _userLoginRepository.FirstOrDefault(
                ul => ul.LoginProvider == login.LoginProvider && ul.ProviderKey == login.ProviderKey
            );

            if (userLogin == null)
            {
                return null;
            }

            return UserRepository.FirstOrDefault(u => u.Id == userLogin.UserId);
        }

        [UnitOfWork]
        public virtual Task<List<TUser>> FindAllAsync(UserLoginInfo login)
        {
            var query = from userLogin in _userLoginRepository.GetAll()
                        join user in UserRepository.GetAll() on userLogin.UserId equals user.Id
                        where userLogin.LoginProvider == login.LoginProvider && userLogin.ProviderKey == login.ProviderKey
                        select user;

            return Task.FromResult(query.ToList());
        }

        [UnitOfWork]
        public virtual List<TUser> FindAll(UserLoginInfo login)
        {
            var query = from userLogin in _userLoginRepository.GetAll()
                        join user in UserRepository.GetAll() on userLogin.UserId equals user.Id
                        where userLogin.LoginProvider == login.LoginProvider && userLogin.ProviderKey == login.ProviderKey
                        select user;

            return query.ToList();
        }

        [UnitOfWork]
        public virtual Task<TUser> FindAsync(int? tenantId, UserLoginInfo login)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var query = from userLogin in _userLoginRepository.GetAll()
                            join user in UserRepository.GetAll() on userLogin.UserId equals user.Id
                            where userLogin.LoginProvider == login.LoginProvider && userLogin.ProviderKey == login.ProviderKey
                            select user;

                return Task.FromResult(query.FirstOrDefault());
            }
        }

        [UnitOfWork]
        public virtual TUser Find(int? tenantId, UserLoginInfo login)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var query = from userLogin in _userLoginRepository.GetAll()
                            join user in UserRepository.GetAll() on userLogin.UserId equals user.Id
                            where userLogin.LoginProvider == login.LoginProvider && userLogin.ProviderKey == login.ProviderKey
                            select user;

                return query.FirstOrDefault();
            }
        }

        public async Task<string> GetUserNameFromDatabaseAsync(long userId)
        {
            using (var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                Scope = TransactionScopeOption.Suppress
            }))
            {
                var user = await UserRepository.GetAsync(userId);
                await uow.CompleteAsync();
                return user.UserName;
            }
        }

        public string GetUserNameFromDatabase(long userId)
        {
            using (var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                Scope = TransactionScopeOption.Suppress
            }))
            {
                var user = UserRepository.Get(userId);
                uow.Complete();
                return user.UserName;
            }
        }


        private const string TokenValidityKeyProvider = "TokenValidityKeyProvider";

        public virtual async Task AddTokenValidityKeyAsync([NotNull] TUser user, string tokenValidityKey, DateTime expireDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);

            user.Tokens.Add(new JTUserToken(user, TokenValidityKeyProvider, tokenValidityKey, null, expireDate));
        }

        public virtual void AddTokenValidityKey([NotNull] TUser user, string tokenValidityKey, DateTime expireDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            UserRepository.EnsureCollectionLoaded(user, u => u.Tokens, cancellationToken);

            user.Tokens.Add(new JTUserToken(user, TokenValidityKeyProvider, tokenValidityKey, null, expireDate));
        }

        public virtual async Task<bool> IsTokenValidityKeyValidAsync([NotNull] TUser user, string tokenValidityKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);

            return user.Tokens.Any(t => t.LoginProvider == TokenValidityKeyProvider &&
                                        t.Name == tokenValidityKey &&
                                        t.ExpireDate > DateTime.UtcNow);
        }

        public virtual bool IsTokenValidityKeyValid([NotNull] TUser user, string tokenValidityKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            UserRepository.EnsureCollectionLoaded(user, u => u.Tokens, cancellationToken);
            var isValidityKeyValid = user.Tokens.Any(t => t.LoginProvider == TokenValidityKeyProvider &&
                                               t.Name == tokenValidityKey &&
                                               t.ExpireDate > DateTime.UtcNow);

            user.Tokens.RemoveAll(t => t.LoginProvider == TokenValidityKeyProvider && t.ExpireDate <= DateTime.UtcNow);

            return isValidityKeyValid;
        }

        public virtual async Task RemoveTokenValidityKeyAsync([NotNull] TUser user, string tokenValidityKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);

            user.Tokens.RemoveAll(t => t.LoginProvider == TokenValidityKeyProvider && t.Name == tokenValidityKey);
        }

        public virtual void RemoveTokenValidityKey([NotNull] TUser user, string tokenValidityKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            UserRepository.EnsureCollectionLoaded(user, u => u.Tokens, cancellationToken);

            user.Tokens.Remove(user.Tokens.FirstOrDefault(t =>
                t.LoginProvider == TokenValidityKeyProvider && t.Name == tokenValidityKey));
        }

        protected virtual string NormalizeKey(string key)
        {
            return key.ToUpperInvariant();
        }

        public void Dispose()
        {
            //No need to dispose since using IOC.
        }
    }
}
