using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.Authorization;
using Abp.Castle.Logging.Log4Net;
using Abp.EntityFrameworkCore;
using Abp.MultiTenancy;
using Castle.Facilities.Logging;
using JT.Abp.Application.Editions;
using JT.Abp.Authorization;
using JT.Abp.Authorization.Roles;
using JT.Abp.Authorization.Users;
using JT.Abp.Identity;
using JT.Abp.MultiTenancy;
using JT.Authorization;
using JT.Authorization.Roles;
using JT.Authorization.Users;
using JT.Configuration;
using JT.EntityFrameworkCore;
using JT.MultiTenancy;
using JT.Web.Core.StartupConfigurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace JT.Web.Startup
{
    public class Startup
    {
        private readonly IConfigurationRoot _appConfiguration;

        public Startup(IWebHostEnvironment env)
        {
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName, env.IsDevelopment());
        }
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //Configure DbContext
            services.AddAbpDbContext<JTDbContext>(options =>
            {
                DbContextOptionsConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
            });

            services.AddJTIdentity<Tenant, User, Role>()
                .AddJTTenantManager<JTTenantManager<Tenant, User>>()
                .AddJTUserManager<UserManager>()
                .AddJTRoleManager<RoleManager>()
                .AddJTEditionManager<JTEditionManager>()
                .AddJTUserStore<UserStore>()
                .AddJTUserStore<JTUserStore<Role, User>>()
                .AddJTRoleStore<RoleStore>()
                .AddJTLogInManager<JTLogInManager<Tenant, Role, User>>()
                .AddJTSignInManager<JTSignInManager<Tenant, Role, User>>()
                .AddJTSecurityStampValidator<JTSecurityStampValidator<Tenant, Role, User>>()
                .AddJTUserClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
                .AddJTPermissionChecker<PermissionChecker>()
                .AddDefaultTokenProviders();

            CorsConfigurer.Configure(services);
            AuthConfigurer.Configure(services, _appConfiguration);
            MvcConfigurer.Configure(services);
            SwaggerConfigurer.Configure(services, _appConfiguration);

            services.AddControllersWithViews(options =>
            {
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());
            }).AddNewtonsoftJson();

            //Configure Abp and Dependency Injection
            return services.AddAbp<JTWebModule>(options =>
            {
                //Configure Log4Net logging
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                );
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAbp(options => { options.UseAbpRequestLocalization = false; }); //Initializes ABP framework.

            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            //app.UseJwtTokenMiddleware();

            app.UseAuthorization();

            CorsConfigurer.Use(app);
            MvcConfigurer.Use(app);
            SwaggerConfigurer.Use(app, _appConfiguration);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
