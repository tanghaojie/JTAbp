using System;
using System.Collections.Generic;

namespace JT.Abp.MultiTenancy
{
    [Serializable]
    public class JTTenantFeatureCacheItem
    {
        public const string CacheStoreName = "JTTenantFeatures";

        public int? EditionId { get; set; }

        public IDictionary<string, string> FeatureValues { get; private set; }

        public JTTenantFeatureCacheItem()
        {
            FeatureValues = new Dictionary<string, string>();
        }
    }
}
