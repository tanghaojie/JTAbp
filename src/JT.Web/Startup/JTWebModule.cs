using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using JT.Configuration;
using JT.EntityFrameworkCore;
using JT.Web.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;

namespace JT.Web.Startup
{
    [DependsOn(
        typeof(JTApplicationModule), 
        typeof(JTEntityFrameworkCoreModule), 
        typeof(JTWebCoreModule),
        typeof(AbpAspNetCoreModule))]
    public class JTWebModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public JTWebModule(IHostingEnvironment env)
        {
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName);
        }

        public override void PreInitialize()
        {
            //Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(JTConsts.ConnectionStringName);

            //Configuration.Navigation.Providers.Add<JTNavigationProvider>();

            //Configuration.Modules.AbpAspNetCore()
            //    .CreateControllersForAppServices(
            //        typeof(JTApplicationModule).GetAssembly()
            //    );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JTWebModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            //IocManager.Resolve<ApplicationPartManager>()
            //    .AddApplicationPartsIfNotAddedBefore(typeof(JTWebModule).Assembly);
        }
    }
}