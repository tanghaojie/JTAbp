using Abp.Runtime.Caching;
using JT.Abp.Authorization.Roles;
using JT.Abp.Common.Application.Editions;
using JT.Abp.Common.Authorization.Users;
using JT.Abp.MultiTenancy;

namespace JT.Abp.Runtime
{
    public static class JTCacheManagerExtensions
    {
        public static ITypedCache<string, JTUserPermissionCacheItem> GetUserPermissionCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, JTUserPermissionCacheItem>(JTUserPermissionCacheItem.CacheStoreName);
        }

        public static ITypedCache<string, JTRolePermissionCacheItem> GetRolePermissionCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, JTRolePermissionCacheItem>(JTRolePermissionCacheItem.CacheStoreName);
        }

        public static ITypedCache<int, JTTenantFeatureCacheItem> GetTenantFeatureCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<int, JTTenantFeatureCacheItem>(JTTenantFeatureCacheItem.CacheStoreName);
        }

        public static ITypedCache<int, JTEditionfeatureCacheItem> GetEditionFeatureCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<int, JTEditionfeatureCacheItem>(JTEditionfeatureCacheItem.CacheStoreName);
        }
    }
}
