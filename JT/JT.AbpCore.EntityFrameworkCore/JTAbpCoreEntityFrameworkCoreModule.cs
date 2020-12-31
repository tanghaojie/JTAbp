using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.Reflection.Extensions;
using JT.Abp.Core;

namespace JT.AbpCore.EntityFrameworkCore
{
    [DependsOn(typeof(JTAbpCoreModule), typeof(AbpEntityFrameworkCoreModule))]
    public class JTAbpCoreEntityFrameworkCoreModule : AbpModule
    {
        public override void PreInitialize()
        {

        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JTAbpCoreEntityFrameworkCoreModule).GetAssembly());
        }
    }
}
