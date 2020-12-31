using Abp.MultiTenancy;
using System;

namespace JT.Abp.Common.Tenancy
{
    public class TenantStore : ITenantStore
    {
        public TenantStore()
        {

        }

        public TenantInfo Find(int tenantId)
        {
            throw new Exception("Multi tenant store do implement yet.");
        }

        public TenantInfo Find(string tenancyName)
        {
            throw new Exception("Multi tenant store do implement yet.");
        }
    }
}
