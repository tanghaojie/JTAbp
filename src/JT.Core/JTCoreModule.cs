using Abp.Modules;
using Abp.Reflection.Extensions;
using JT.Abp.Configuration;
using JT.Abp.Core;
using JT.Authorization;
using JT.Authorization.Roles;
using JT.Authorization.Users;
using JT.Localization;
using JT.MultiTenancy;

namespace JT
{
    [DependsOn(typeof(JTAbpCoreModule))]
    public class JTCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.JT().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.JT().EntityTypes.Role = typeof(Role);
            Configuration.Modules.JT().EntityTypes.User = typeof(User);

            Configuration.Authorization.Providers.Add<JTAuthorizationProvider>();

            Configuration.Auditing.IsEnabledForAnonymousUsers = true;
            Configuration.MultiTenancy.IsEnabled = false;
            Configuration.Authorization.IsEnabled = true;

            Configuration.Localization.IsEnabled = false;

            JTLocalizationConfigurer.Configure(Configuration.Localization);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JTCoreModule).GetAssembly());
        }
    }
}