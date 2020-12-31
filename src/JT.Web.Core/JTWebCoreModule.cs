using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.SignalR;
using Abp.Modules;
using JT.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Reflection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using JT.Configuration;

namespace JT.Web.Core
{
    [DependsOn(
       typeof(JTApplicationModule),
       typeof(JTEntityFrameworkCoreModule),
       typeof(AbpAspNetCoreModule)
      , typeof(AbpAspNetCoreSignalRModule)
   )]
    public class JTWebCoreModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public JTWebCoreModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                JTConsts.ConnectionStringName
            );

            // Use database for language management
            //Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(JTApplicationModule).GetAssembly()
                 );

            ConfigureTokenAuth();

            IocManager.Register<JTAbpExceptionFilter>();
        }

        private void ConfigureTokenAuth()
        {
            //IocManager.Register<TokenAuthConfiguration>();
            //var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

            //tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            //tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
            //tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
            //tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            //tokenAuthConfig.Expiration = TimeSpan.FromDays(1);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JTWebCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(JTWebCoreModule).Assembly);
        }
    }
}
