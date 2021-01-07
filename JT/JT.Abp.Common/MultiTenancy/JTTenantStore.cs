using Abp.MultiTenancy;

namespace JT.Abp.MultiTenancy
{
    public class JTTenantStore : ITenantStore
    {
        private readonly IJTTenantCache _tenantCache;

        public JTTenantStore(IJTTenantCache tenantCache)
        {
            _tenantCache = tenantCache;
        }

        public TenantInfo Find(int tenantId)
        {
            var tenant = _tenantCache.GetOrNull(tenantId);
            if (tenant == null)
            {
                return null;
            }

            return new TenantInfo(tenant.Id, tenant.TenancyName);
        }

        public TenantInfo Find(string tenancyName)
        {
            var tenant = _tenantCache.GetOrNull(tenancyName);
            if (tenant == null)
            {
                return null;
            }

            return new TenantInfo(tenant.Id, tenant.TenancyName);
        }
    }
}
