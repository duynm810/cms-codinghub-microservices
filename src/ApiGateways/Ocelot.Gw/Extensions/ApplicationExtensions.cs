using Ocelot.Middleware;

namespace Ocelot.Gw.Extensions;

public static class ApplicationExtensions
{
    /// <summary>
    /// Configures the HTTP request pipeline with essential middleware components such as Swagger UI, routing, and endpoint mapping.
    /// </summary>
    /// <param name="app">The IApplicationBuilder to configure the middleware pipeline.</param>
    public static async void ConfigurePipeline(this IApplicationBuilder app)
    {
        app.UseCors("CorsPolicy");

        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();

        /*app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", context =>
            {
                context.Response.Redirect("swagger/index.html");
                return Task.CompletedTask;
            });
        });

        app.UseSwaggerForOcelotUI(
            opt => { opt.PathToSwaggerGenerator = "/swagger/docs"; });
            */

        await app.UseOcelot();
    }
}