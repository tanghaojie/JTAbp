using Abp.Configuration.Startup;

namespace JT.Abp.Configuration
{
    public static class ModuleJTConfigurationExtensions
    {
        public static IJTConfig JT(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.AbpConfiguration.Get<IJTConfig>();
        }
    }
}
