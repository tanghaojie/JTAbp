using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using JT.Authorization;

namespace JT
{
    [DependsOn(
        typeof(JTCoreModule),
        typeof(AbpAutoMapperModule))]
    public class JTApplicationModule : AbpModule
    {
        public override void PostInitialize()
        {
           
        }
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