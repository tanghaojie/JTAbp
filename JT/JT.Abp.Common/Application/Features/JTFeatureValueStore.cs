﻿using Abp.Application.Features;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Localization;
using Abp.Runtime.Caching;
using Abp.UI;
using JT.Abp.Application.Editions;
using JT.Abp.Authorization.Users;
using JT.Abp.Common.Application.Editions;
using JT.Abp.MultiTenancy;
using JT.Abp.Runtime;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace JT.Abp.Application.Features
{
    public class JTFeatureValueStore<TTenant, TUser> :
        IJTFeatureValueStore,
        ITransientDependency,
        IEventHandler<EntityChangingEventData<JTEdition>>,
        IEventHandler<EntityChangingEventData<JTEditionFeature>>,
        IEventHandler<EntityChangingEventData<JTTenantFeature>>

        where TTenant : JTTenant<TUser>
        where TUser : JTUserBase
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<JTTenantFeature, long> _tenantFeatureRepository;
        private readonly IRepository<TTenant> _tenantRepository;
        private readonly IRepository<JTEditionFeature, long> _editionFeatureRepository;
        private readonly IFeatureManager _featureManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ILocalizationManager LocalizationManager { get; set; }
        protected string LocalizationSourceName { get; set; }

        public JTFeatureValueStore(
            ICacheManager cacheManager,
            IRepository<JTTenantFeature, long> tenantFeatureRepository,
            IRepository<TTenant> tenantRepository,
            IRepository<JTEditionFeature, long> editionFeatureRepository,
            IFeatureManager featureManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _cacheManager = cacheManager;
            _tenantFeatureRepository = tenantFeatureRepository;
            _tenantRepository = tenantRepository;
            _editionFeatureRepository = editionFeatureRepository;
            _featureManager = featureManager;
            _unitOfWorkManager = unitOfWorkManager;

            LocalizationManager = NullLocalizationManager.Instance;
            //LocalizationSourceName = AbpZeroConsts.LocalizationSourceName;
        }

        public virtual Task<string> GetValueOrNullAsync(int tenantId, Feature feature)
        {
            return GetValueOrNullAsync(tenantId, feature.Name);
        }

        public virtual string GetValueOrNull(int tenantId, Feature feature)
        {
            return GetValueOrNull(tenantId, feature.Name);
        }

        public virtual async Task<string> GetEditionValueOrNullAsync(int editionId, string featureName)
        {
            var cacheItem = await GetEditionFeatureCacheItemAsync(editionId);
            return cacheItem.FeatureValues.GetOrDefault(featureName);
        }

        public virtual string GetEditionValueOrNull(int editionId, string featureName)
        {
            var cacheItem = GetEditionFeatureCacheItem(editionId);
            return cacheItem.FeatureValues.GetOrDefault(featureName);
        }

        public virtual async Task<string> GetValueOrNullAsync(int tenantId, string featureName)
        {
            var cacheItem = await GetTenantFeatureCacheItemAsync(tenantId);
            var value = cacheItem.FeatureValues.GetOrDefault(featureName);
            if (value != null)
            {
                return value;
            }

            if (cacheItem.EditionId.HasValue)
            {
                value = await GetEditionValueOrNullAsync(cacheItem.EditionId.Value, featureName);
                if (value != null)
                {
                    return value;
                }
            }

            return null;
        }

        public virtual string GetValueOrNull(int tenantId, string featureName)
        {
            var cacheItem = GetTenantFeatureCacheItem(tenantId);
            var value = cacheItem.FeatureValues.GetOrDefault(featureName);
            if (value != null)
            {
                return value;
            }

            if (cacheItem.EditionId.HasValue)
            {
                value = GetEditionValueOrNull(cacheItem.EditionId.Value, featureName);
                if (value != null)
                {
                    return value;
                }
            }

            return null;
        }

        [UnitOfWork]
        public virtual async Task SetEditionFeatureValueAsync(int editionId, string featureName, string value)
        {
            using (_unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant))
            using (_unitOfWorkManager.Current.SetTenantId(null))
            {
                if (await GetEditionValueOrNullAsync(editionId, featureName) == value)
                {
                    return;
                }

                var currentFeature = await _editionFeatureRepository.FirstOrDefaultAsync(f => f.EditionId == editionId && f.Name == featureName);

                var feature = _featureManager.GetOrNull(featureName);
                if (feature == null || feature.DefaultValue == value)
                {
                    if (currentFeature != null)
                    {
                        await _editionFeatureRepository.DeleteAsync(currentFeature);
                    }

                    return;
                }

                if (!feature.InputType.Validator.IsValid(value))
                {
                    throw new UserFriendlyException(string.Format(
                        L("InvalidFeatureValue"), feature.Name));
                }

                if (currentFeature == null)
                {
                    await _editionFeatureRepository.InsertAsync(new JTEditionFeature(editionId, featureName, value));
                }
                else
                {
                    currentFeature.Value = value;
                }
            }
        }

        [UnitOfWork]
        public virtual void SetEditionFeatureValue(int editionId, string featureName, string value)
        {
            using (_unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant))
            using (_unitOfWorkManager.Current.SetTenantId(null))
            {
                if (GetEditionValueOrNull(editionId, featureName) == value)
                {
                    return;
                }

                var currentFeature = _editionFeatureRepository.FirstOrDefault(f => f.EditionId == editionId && f.Name == featureName);

                var feature = _featureManager.GetOrNull(featureName);
                if (feature == null || feature.DefaultValue == value)
                {
                    if (currentFeature != null)
                    {
                        _editionFeatureRepository.Delete(currentFeature);
                    }

                    return;
                }

                if (currentFeature == null)
                {
                    _editionFeatureRepository.Insert(new JTEditionFeature(editionId, featureName, value));
                }
                else
                {
                    currentFeature.Value = value;
                }
            }
        }

        protected virtual async Task<JTTenantFeatureCacheItem> GetTenantFeatureCacheItemAsync(int tenantId)
        {
            return await _cacheManager.GetTenantFeatureCache().GetAsync(tenantId, async () =>
            {
                TTenant tenant;
                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(null))
                    {
                        tenant = await _tenantRepository.GetAsync(tenantId);

                        await uow.CompleteAsync();
                    }
                }

                var newCacheItem = new JTTenantFeatureCacheItem { EditionId = tenant.EditionId };

                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant))
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var featureSettings = await _tenantFeatureRepository.GetAllListAsync();
                        foreach (var featureSetting in featureSettings)
                        {
                            newCacheItem.FeatureValues[featureSetting.Name] = featureSetting.Value;
                        }

                        await uow.CompleteAsync();
                    }
                }

                return newCacheItem;
            });
        }

        protected virtual JTTenantFeatureCacheItem GetTenantFeatureCacheItem(int tenantId)
        {
            return _cacheManager.GetTenantFeatureCache().Get(tenantId, () =>
            {
                TTenant tenant;
                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(null))
                    {
                        tenant = _tenantRepository.Get(tenantId);

                        uow.Complete();
                    }
                }

                var newCacheItem = new JTTenantFeatureCacheItem { EditionId = tenant.EditionId };

                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant))
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var featureSettings = _tenantFeatureRepository.GetAllList();
                        foreach (var featureSetting in featureSettings)
                        {
                            newCacheItem.FeatureValues[featureSetting.Name] = featureSetting.Value;
                        }

                        uow.Complete();
                    }
                }

                return newCacheItem;
            });
        }

        protected virtual async Task<JTEditionfeatureCacheItem> GetEditionFeatureCacheItemAsync(int editionId)
        {
            return await _cacheManager
                .GetEditionFeatureCache()
                .GetAsync(
                    editionId,
                    async () => await CreateEditionFeatureCacheItemAsync(editionId)
                );
        }

        protected virtual JTEditionfeatureCacheItem GetEditionFeatureCacheItem(int editionId)
        {
            return _cacheManager
                .GetEditionFeatureCache()
                .Get(
                    editionId,
                    () => CreateEditionFeatureCacheItem(editionId)
                );
        }

        protected virtual async Task<JTEditionfeatureCacheItem> CreateEditionFeatureCacheItemAsync(int editionId)
        {
            var newCacheItem = new JTEditionfeatureCacheItem();

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant))
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    var featureSettings = await _editionFeatureRepository.GetAllListAsync(f => f.EditionId == editionId);
                    foreach (var featureSetting in featureSettings)
                    {
                        newCacheItem.FeatureValues[featureSetting.Name] = featureSetting.Value;
                    }

                    await uow.CompleteAsync();
                }
            }

            return newCacheItem;
        }

        protected virtual JTEditionfeatureCacheItem CreateEditionFeatureCacheItem(int editionId)
        {
            var newCacheItem = new JTEditionfeatureCacheItem();

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant))
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    var featureSettings = _editionFeatureRepository.GetAllList(f => f.EditionId == editionId);
                    foreach (var featureSetting in featureSettings)
                    {
                        newCacheItem.FeatureValues[featureSetting.Name] = featureSetting.Value;
                    }

                    uow.Complete();
                }
            }

            return newCacheItem;
        }

        public virtual void HandleEvent(EntityChangingEventData<JTEditionFeature> eventData)
        {
            _cacheManager.GetEditionFeatureCache().Remove(eventData.Entity.EditionId);
        }

        public virtual void HandleEvent(EntityChangingEventData<JTEdition> eventData)
        {
            if (eventData.Entity.IsTransient())
            {
                return;
            }

            _cacheManager.GetEditionFeatureCache().Remove(eventData.Entity.Id);
        }

        public virtual void HandleEvent(EntityChangingEventData<JTTenantFeature> eventData)
        {
            if (eventData.Entity.TenantId.HasValue)
            {
                _cacheManager.GetTenantFeatureCache().Remove(eventData.Entity.TenantId.Value);
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
    }
}
