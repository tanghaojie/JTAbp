using Abp.Modules;
using Abp.Reflection.Extensions;
using JT.Localization;

namespace JT
{
    public class JTCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            JTLocalizationConfigurer.Configure(Configuration.Localization);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JTCoreModule).GetAssembly());
        }
    }
}