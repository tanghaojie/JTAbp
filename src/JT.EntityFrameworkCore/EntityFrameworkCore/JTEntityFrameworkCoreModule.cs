using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace JT.EntityFrameworkCore
{
    [DependsOn(
        typeof(JTCoreModule), 
        typeof(AbpEntityFrameworkCoreModule))]
    public class JTEntityFrameworkCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JTEntityFrameworkCoreModule).GetAssembly());
        }
    }
}