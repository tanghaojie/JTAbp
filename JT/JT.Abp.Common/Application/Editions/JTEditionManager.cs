using Abp;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Caching;
using JT.Abp.Application.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Collections.Extensions;
using System.Threading.Tasks;

namespace JT.Abp.Application.Editions
{
    public class JTEditionManager : IDomainService
    {
        private readonly IJTFeatureValueStore _featureValueStore;

        public IQueryable<JTEdition> Editions => EditionRepository.GetAll();

        public ICacheManager CacheManager { get; set; }

        public IFeatureManager FeatureManager { get; set; }

        protected IRepository<JTEdition> EditionRepository { get; set; }

        public JTEditionManager(
            IRepository<JTEdition> editionRepository,
            IJTFeatureValueStore featureValueStore)
        {
            _featureValueStore = featureValueStore;
            EditionRepository = editionRepository;
        }

        public virtual Task<string> GetFeatureValueOrNullAsync(int editionId, string featureName)
        {
            return _featureValueStore.GetEditionValueOrNullAsync(editionId, featureName);
        }

        public virtual string GetFeatureValueOrNull(int editionId, string featureName)
        {
            return _featureValueStore.GetEditionValueOrNull(editionId, featureName);
        }

        public virtual Task SetFeatureValueAsync(int editionId, string featureName, string value)
        {
            return _featureValueStore.SetEditionFeatureValueAsync(editionId, featureName, value);
        }

        public virtual void SetFeatureValue(int editionId, string featureName, string value)
        {
            _featureValueStore.SetEditionFeatureValue(editionId, featureName, value);
        }

        public virtual async Task<IReadOnlyList<NameValue>> GetFeatureValuesAsync(int editionId)
        {
            var values = new List<NameValue>();

            foreach (var feature in FeatureManager.GetAll())
            {
                values.Add(new NameValue(feature.Name, await GetFeatureValueOrNullAsync(editionId, feature.Name) ?? feature.DefaultValue));
            }

            return values;
        }

        public virtual IReadOnlyList<NameValue> GetFeatureValues(int editionId)
        {
            var values = new List<NameValue>();

            foreach (var feature in FeatureManager.GetAll())
            {
                values.Add(new NameValue(feature.Name, GetFeatureValueOrNull(editionId, feature.Name) ?? feature.DefaultValue));
            }

            return values;
        }

        public virtual async Task SetFeatureValuesAsync(int editionId, params NameValue[] values)
        {
            if (values.IsNullOrEmpty())
            {
                return;
            }

            foreach (var value in values)
            {
                await SetFeatureValueAsync(editionId, value.Name, value.Value);
            }
        }

        public virtual void SetFeatureValues(int editionId, params NameValue[] values)
        {
            if (values.IsNullOrEmpty())
            {
                return;
            }

            foreach (var value in values)
            {
                SetFeatureValue(editionId, value.Name, value.Value);
            }
        }

        public virtual Task CreateAsync(JTEdition edition)
        {
            return EditionRepository.InsertAsync(edition);
        }

        public virtual void Create(JTEdition edition)
        {
            EditionRepository.Insert(edition);
        }

        public virtual Task<JTEdition> FindByNameAsync(string name)
        {
            return EditionRepository.FirstOrDefaultAsync(edition => edition.Name == name);
        }

        public virtual JTEdition FindByName(string name)
        {
            return EditionRepository.FirstOrDefault(edition => edition.Name == name);
        }

        public virtual Task<JTEdition> FindByIdAsync(int id)
        {
            return EditionRepository.FirstOrDefaultAsync(id);
        }

        public virtual JTEdition FindById(int id)
        {
            return EditionRepository.FirstOrDefault(id);
        }

        public virtual Task<JTEdition> GetByIdAsync(int id)
        {
            return EditionRepository.GetAsync(id);
        }

        public virtual JTEdition GetById(int id)
        {
            return EditionRepository.Get(id);
        }

        public virtual Task DeleteAsync(JTEdition edition)
        {
            return EditionRepository.DeleteAsync(edition);
        }

        public virtual void Delete(JTEdition edition)
        {
            EditionRepository.Delete(edition);
        }
    }
}
