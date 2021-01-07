using System;

namespace JT.Abp.MultiTenancy
{
    [Serializable]
    public class JTTenantCacheItem
    {
        public const string CacheName = "JTTenantCache";

        public const string ByNameCacheName = "JTTenantByNameCache";

        public int Id { get; set; }

        public string Name { get; set; }

        public string TenancyName { get; set; }

        public string ConnectionString { get; set; }

        public int? EditionId { get; set; }

        public bool IsActive { get; set; }

        public object CustomData { get; set; }
    }
}
