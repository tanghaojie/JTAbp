using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.Reflection.Extensions;
using JT.AbpCore.EntityFrameworkCore;

namespace JT.EntityFrameworkCore
{
    [DependsOn(
        typeof(JTAbpCoreEntityFrameworkCoreModule), 
        typeof(AbpEntityFrameworkCoreModule))]
    public class JTEntityFrameworkCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JTEntityFrameworkCoreModule).GetAssembly());
        }
    }
}