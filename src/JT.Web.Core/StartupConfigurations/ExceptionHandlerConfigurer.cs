using Microsoft.AspNetCore.Builder;

namespace JT.Web.Core.StartupConfigurations
{
    public class ExceptionHandlerConfigurer
    {
        public static void UseDev(IApplicationBuilder app)
        {
            //app.UseExceptionHandler(
            //    options =>
            //    {
            //        options.Run(async context =>
            //        {
            //            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //            context.Response.ContentType = "text/html";
            //            var ex = context.Features.Get<IExceptionHandlerFeature>();
            //            if (ex != null)
            //            {
            //                var err = $"<h1>Error: {ex.Error.Message}</h1>{ex.Error.StackTrace}";
            //                await context.Response.WriteAsync(err).ConfigureAwait(false);
            //            }
            //        });
            //    });
            app.UseDeveloperExceptionPage();
        }
    }
}
