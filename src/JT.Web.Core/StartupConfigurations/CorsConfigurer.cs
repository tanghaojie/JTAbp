using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace JT.Web.Core.StartupConfigurations
{
    public class CorsConfigurer
    {
        private const string DEFAULT_CORS_POLICY_NAME = "localhost";

        public static void Configure(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(DEFAULT_CORS_POLICY_NAME, builder =>
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
            app.UseCors(DEFAULT_CORS_POLICY_NAME);
        }
    }
}
