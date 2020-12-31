using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace JT
{
    [DependsOn(
        typeof(JTCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class JTApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JTApplicationModule).GetAssembly());
        }
    }
}