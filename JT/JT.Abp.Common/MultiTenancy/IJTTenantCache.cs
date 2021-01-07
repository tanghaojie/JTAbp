namespace JT.Abp.MultiTenancy
{
    public interface IJTTenantCache
    {
        JTTenantCacheItem Get(int tenantId);

        JTTenantCacheItem Get(string tenancyName);

        JTTenantCacheItem GetOrNull(string tenancyName);

        JTTenantCacheItem GetOrNull(int tenantId);
    }
}
