using Abp.MultiTenancy;

namespace JT.Abp.Common.Tenancy
{
    public class NoneTenantStore : ITenantStore
    {
        private static readonly TenantInfo NoneTenantInfo = new(NoneTenancyConsts.DefaultTenantId, NoneTenancyConsts.DefaultTenantName);
        public NoneTenantStore() { }
        public TenantInfo Find(int tenantId)
        {
            return tenantId == NoneTenancyConsts.DefaultTenantId ? NoneTenantInfo : null;
        }
        public TenantInfo Find(string tenancyName)
        {
            return tenancyName == NoneTenancyConsts.DefaultTenantName ? NoneTenantInfo : null;
        }
    }
}
