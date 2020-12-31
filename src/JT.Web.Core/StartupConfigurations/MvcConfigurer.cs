using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace JT.Web.Core.StartupConfigurations
{
    public class MvcConfigurer
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.AddService(typeof(JTAbpExceptionFilter), 1);
                options.Filters.Add(new IgnoreAntiforgeryTokenAttribute());
            });
        }

        public static void Use(IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
