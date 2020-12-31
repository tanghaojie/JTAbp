using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace JT.Web.Core.StartupConfigurations
{
    public class CorsConfigurer
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddCors(config =>
            {
                config.AddPolicy("policy", builder =>
                {
                    builder
                    //.AllowAnyOrigin()
                    //.WithOrigins(new string[] { "http://127.0.0.1:5500" })
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });
        }

        public static void Use(IApplicationBuilder app)
        {
            app.UseCors("policy");
        }
    }
}
