using JT.Abp.Application.Features;

namespace JT.Abp.MultiTenancy
{
    public class JTTenantFeature : JTFeature
    {
        public JTTenantFeature()
        {
        }

        public JTTenantFeature(int tenantId, string name, string value)
            : base(name, value)
        {
            TenantId = tenantId;
        }
    }
}
