using Abp.Modules;
using Abp.Reflection.Extensions;
using JT.Abp.Common;

namespace JT.Abp.Core
{
    [DependsOn(typeof(JTAbpCommonModule))]
    public class JTAbpCoreModule : AbpModule
    {
        public override void PreInitialize()
        {

        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JTAbpCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {

        }
    }
}
