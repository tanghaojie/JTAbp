using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JT.Web.Core.StartupConfigurations
{
    public class CookieConfigurer
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            if (bool.Parse(configuration["Cookie:IsEnabled"]))
            {
                var name = configuration["Cookie:Name"];
                var expireInMinute = int.Parse(configuration["Cookie:ExpireInMinute"]);

                services.Configure<CookiePolicyOptions>(options =>
                {
                    //options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });
                services
                    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.Cookie.Name = name;
                        options.ExpireTimeSpan = new TimeSpan(0, expireInMinute, 0);
                    });
            }
        }

        public static void Use(IApplicationBuilder app)
        {
            app.UseAuthorization();
            app.UseCookiePolicy();
        }
    }
}
