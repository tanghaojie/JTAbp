using Abp;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Localization;
using Abp.Runtime.Caching;
using Abp.UI;
using JT.Abp.Application.Editions;
using JT.Abp.Application.Features;
using JT.Abp.Authorization.Users;
using JT.Abp.Runtime;
using Abp.Collections.Extensions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JT.Abp.MultiTenancy
{
    public class JTTenantManager<TTenant, TUser> : IDomainService,
        IEventHandler<EntityChangedEventData<TTenant>>,
        IEventHandler<EntityDeletedEventData<JTEdition>>
        where TTenant : JTTenant<TUser>
        where TUser : JTUserBase
    {
        public JTEditionManager EditionManager { get; set; }

        public ILocalizationManager LocalizationManager { get; set; }

        protected string LocalizationSourceName { get; set; }

        public ICacheManager CacheManager { get; set; }

        public IFeatureManager FeatureManager { get; set; }

        public IUnitOfWorkManager UnitOfWorkManager { get; set; }

        protected IRepository<TTenant> TenantRepository { get; set; }

        protected IRepository<JTTenantFeature, long> TenantFeatureRepository { get; set; }

        private readonly IJTFeatureValueStore _featureValueStore;

        public JTTenantManager(
            IRepository<TTenant> tenantRepository,
            IRepository<JTTenantFeature, long> tenantFeatureRepository,
            JTEditionManager editionManager,
            IJTFeatureValueStore featureValueStore)
        {
            _featureValueStore = featureValueStore;
            TenantRepository = tenantRepository;
            TenantFeatureRepository = tenantFeatureRepository;
            EditionManager = editionManager;
            LocalizationManager = NullLocalizationManager.Instance;
            //LocalizationSourceName = AbpZeroConsts.LocalizationSourceName;
        }

        public virtual IQueryable<TTenant> Tenants { get { return TenantRepository.GetAll(); } }

        public virtual async Task CreateAsync(TTenant tenant)
        {
            await ValidateTenantAsync(tenant);

            if (await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenant.TenancyName) != null)
            {
                throw new UserFriendlyException(string.Format(L("TenancyNameIsAlreadyTaken"), tenant.TenancyName));
            }

            await TenantRepository.InsertAsync(tenant);
        }

        public virtual void Create(TTenant tenant)
        {
            ValidateTenant(tenant);

            if (TenantRepository.FirstOrDefault(t => t.TenancyName == tenant.TenancyName) != null)
            {
                throw new UserFriendlyException(string.Format(L("TenancyNameIsAlreadyTaken"), tenant.TenancyName));
            }

            TenantRepository.Insert(tenant);
        }

        public virtual async Task UpdateAsync(TTenant tenant)
        {
            if (await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenant.TenancyName && t.Id != tenant.Id) != null)
            {
                throw new UserFriendlyException(string.Format(L("TenancyNameIsAlreadyTaken"), tenant.TenancyName));
            }

            await TenantRepository.UpdateAsync(tenant);
        }

        public virtual void Update(TTenant tenant)
        {
            if (TenantRepository.FirstOrDefault(t => t.TenancyName == tenant.TenancyName && t.Id != tenant.Id) != null)
            {
                throw new UserFriendlyException(string.Format(L("TenancyNameIsAlreadyTaken"), tenant.TenancyName));
            }

            TenantRepository.Update(tenant);
        }

        public virtual async Task<TTenant> FindByIdAsync(int id)
        {
            return await TenantRepository.FirstOrDefaultAsync(id);
        }

        public virtual TTenant FindById(int id)
        {
            return TenantRepository.FirstOrDefault(id);
        }

        public virtual async Task<TTenant> GetByIdAsync(int id)
        {
            var tenant = await FindByIdAsync(id);
            if (tenant == null)
            {
                throw new AbpException("There is no tenant with id: " + id);
            }

            return tenant;
        }

        public virtual TTenant GetById(int id)
        {
            var tenant = FindById(id);
            if (tenant == null)
            {
                throw new AbpException("There is no tenant with id: " + id);
            }

            return tenant;
        }

        public virtual Task<TTenant> FindByTenancyNameAsync(string tenancyName)
        {
            return TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenancyName);
        }

        public virtual TTenant FindByTenancyName(string tenancyName)
        {
            return TenantRepository.FirstOrDefault(t => t.TenancyName == tenancyName);
        }

        public virtual async Task DeleteAsync(TTenant tenant)
        {
            await TenantRepository.DeleteAsync(tenant);
        }

        public virtual void Delete(TTenant tenant)
        {
            TenantRepository.Delete(tenant);
        }

        public Task<string> GetFeatureValueOrNullAsync(int tenantId, string featureName)
        {
            return _featureValueStore.GetValueOrNullAsync(tenantId, featureName);
        }

        public string GetFeatureValueOrNull(int tenantId, string featureName)
        {
            return _featureValueStore.GetValueOrNull(tenantId, featureName);
        }

        public virtual async Task<IReadOnlyList<NameValue>> GetFeatureValuesAsync(int tenantId)
        {
            var values = new List<NameValue>();

            foreach (var feature in FeatureManager.GetAll())
            {
                values.Add(new NameValue(feature.Name, await GetFeatureValueOrNullAsync(tenantId, feature.Name) ?? feature.DefaultValue));
            }

            return values;
        }

        public virtual IReadOnlyList<NameValue> GetFeatureValues(int tenantId)
        {
            var values = new List<NameValue>();

            foreach (var feature in FeatureManager.GetAll())
            {
                values.Add(new NameValue(feature.Name, GetFeatureValueOrNull(tenantId, feature.Name) ?? feature.DefaultValue));
            }

            return values;
        }

        public virtual async Task SetFeatureValuesAsync(int tenantId, params NameValue[] values)
        {
            if (values.IsNullOrEmpty())
            {
                return;
            }

            foreach (var value in values)
            {
                await SetFeatureValueAsync(tenantId, value.Name, value.Value);
            }
        }

        public virtual void SetFeatureValues(int tenantId, params NameValue[] values)
        {
            if (values.IsNullOrEmpty())
            {
                return;
            }

            foreach (var value in values)
            {
                SetFeatureValue(tenantId, value.Name, value.Value);
            }
        }

        [UnitOfWork]
        public virtual async Task SetFeatureValueAsync(int tenantId, string featureName, string value)
        {
            await SetFeatureValueAsync(await GetByIdAsync(tenantId), featureName, value);
        }

        [UnitOfWork]
        public virtual void SetFeatureValue(int tenantId, string featureName, string value)
        {
            SetFeatureValue(GetById(tenantId), featureName, value);
        }

        [UnitOfWork]
        public virtual async Task SetFeatureValueAsync(TTenant tenant, string featureName, string value)
        {
            //No need to change if it's already equals to the current value
            if (await GetFeatureValueOrNullAsync(tenant.Id, featureName) == value)
            {
                return;
            }

            //Get the current feature setting
            JTTenantFeature currentSetting;
            using (UnitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant))
            using (UnitOfWorkManager.Current.SetTenantId(tenant.Id))
            {
                currentSetting = await TenantFeatureRepository.FirstOrDefaultAsync(f => f.Name == featureName);
            }

            //Get the feature
            var feature = FeatureManager.GetOrNull(featureName);
            if (feature == null)
            {
                if (currentSetting != null)
                {
                    await TenantFeatureRepository.DeleteAsync(currentSetting);
                }

                return;
            }

            //Determine default value
            var defaultValue = tenant.EditionId.HasValue
                ? (await EditionManager.GetFeatureValueOrNullAsync(tenant.EditionId.Value, featureName) ?? feature.DefaultValue)
                : feature.DefaultValue;

            //No need to store value if it's default
            if (value == defaultValue)
            {
                if (currentSetting != null)
                {
                    await TenantFeatureRepository.DeleteAsync(currentSetting);
                }

                return;
            }

            //Insert/update the feature value
            if (currentSetting == null)
            {
                await TenantFeatureRepository.InsertAsync(new JTTenantFeature(tenant.Id, featureName, value));
            }
            else
            {
                currentSetting.Value = value;
            }
        }

        [UnitOfWork]
        public virtual void SetFeatureValue(TTenant tenant, string featureName, string value)
        {
            //No need to change if it's already equals to the current value
            if (GetFeatureValueOrNull(tenant.Id, featureName) == value)
            {
                return;
            }

            //Get the current feature setting
            JTTenantFeature currentSetting;
            using (UnitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant))
            using (UnitOfWorkManager.Current.SetTenantId(tenant.Id))
            {
                currentSetting = TenantFeatureRepository.FirstOrDefault(f => f.Name == featureName);
            }

            //Get the feature
            var feature = FeatureManager.GetOrNull(featureName);
            if (feature == null)
            {
                if (currentSetting != null)
                {
                    TenantFeatureRepository.Delete(currentSetting);
                }

                return;
            }

            //Determine default value
            var defaultValue = tenant.EditionId.HasValue
                ? (EditionManager.GetFeatureValueOrNull(tenant.EditionId.Value, featureName) ?? feature.DefaultValue)
                : feature.DefaultValue;

            //No need to store value if it's default
            if (value == defaultValue)
            {
                if (currentSetting != null)
                {
                    TenantFeatureRepository.Delete(currentSetting);
                }

                return;
            }

            //Insert/update the feature value
            if (currentSetting == null)
            {
                TenantFeatureRepository.Insert(new JTTenantFeature(tenant.Id, featureName, value));
            }
            else
            {
                currentSetting.Value = value;
            }
        }

        [UnitOfWork]
        public virtual async Task ResetAllFeaturesAsync(int tenantId)
        {
            using (UnitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant))
            using (UnitOfWorkManager.Current.SetTenantId(tenantId))
            {
                await TenantFeatureRepository.DeleteAsync(f => f.TenantId == tenantId);
            }
        }

        [UnitOfWork]
        public virtual void ResetAllFeatures(int tenantId)
        {
            using (UnitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant))
            using (UnitOfWorkManager.Current.SetTenantId(tenantId))
            {
                TenantFeatureRepository.Delete(f => f.TenantId == tenantId);
            }
        }

        protected virtual async Task ValidateTenantAsync(TTenant tenant)
        {
            await ValidateTenancyNameAsync(tenant.TenancyName);
        }

        protected virtual void ValidateTenant(TTenant tenant)
        {
            ValidateTenancyName(tenant.TenancyName);
        }

        protected virtual Task ValidateTenancyNameAsync(string tenancyName)
        {
            if (!Regex.IsMatch(tenancyName, JTTenant<TUser>.TenancyNameRegex))
            {
                throw new UserFriendlyException(L("InvalidTenancyName"));
            }

            return Task.FromResult(0);
        }

        protected virtual void ValidateTenancyName(string tenancyName)
        {
            if (!Regex.IsMatch(tenancyName, JTTenant<TUser>.TenancyNameRegex))
            {
                throw new UserFriendlyException(L("InvalidTenancyName"));
            }
        }

        protected virtual string L(string name)
        {
            return LocalizationManager.GetString(LocalizationSourceName, name);
        }

        protected virtual string L(string name, CultureInfo cultureInfo)
        {
            return LocalizationManager.GetString(LocalizationSourceName, name, cultureInfo);
        }

        public void HandleEvent(EntityChangedEventData<TTenant> eventData)
        {
            if (eventData.Entity.IsTransient())
            {
                return;
            }

            CacheManager.GetTenantFeatureCache().Remove(eventData.Entity.Id);
        }

        [UnitOfWork]
        public virtual void HandleEvent(EntityDeletedEventData<JTEdition> eventData)
        {
            var relatedTenants = TenantRepository.GetAllList(t => t.EditionId == eventData.Entity.Id);
            foreach (var relatedTenant in relatedTenants)
            {
                relatedTenant.EditionId = null;
            }
        }
    }
}
