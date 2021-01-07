using Abp.Runtime.Caching;

namespace JT.Abp.MultiTenancy
{
    public static class JTTenantCacheManagerExtensions
    {
        public static ITypedCache<int, JTTenantCacheItem> GetTenantCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<int, JTTenantCacheItem>(JTTenantCacheItem.CacheName);
        }

        public static ITypedCache<string, int?> GetTenantByNameCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, int?>(JTTenantCacheItem.ByNameCacheName);
        }
    }
}
