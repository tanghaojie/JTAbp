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
            var thisAssembly = typeof(JTApplicationModule).GetAssembly();
            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
               cfg => cfg.AddMaps(thisAssembly)
           );
        }
    }
}