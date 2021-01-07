using System;
using System.Collections.Generic;

namespace JT.Abp.Common.Application.Editions
{
    [Serializable]
    public class JTEditionfeatureCacheItem
    {
        public const string CacheStoreName = "JTEditionFeatures";

        public IDictionary<string, string> FeatureValues { get; set; }

        public JTEditionfeatureCacheItem()
        {
            FeatureValues = new Dictionary<string, string>();
        }
    }
}
