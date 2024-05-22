using Infrastructure.Middlewares;
using Ocelot.Gw.Configs;
using Ocelot.Middleware;

namespace Ocelot.Gw.Extensions;

public static class ApplicationExtensions
{
    /// <summary>
    /// Configures the HTTP request pipeline with essential middleware components such as Swagger UI, routing, and endpoint mapping.
    /// </summary>
    /// <param name="app">The IApplicationBuilder to configure the middleware pipeline.</param>
    public static void ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }
        
        app.UseMiddleware<ErrorWrappingMiddleware>();
        
        // Enables routing in the application.
        app.UseRouting();
        
        app.UseStaticFiles();

        app.UseCors("CorsPolicy");

        app.UseSwagger();

        app.UseSwaggerForOcelotUI(options =>
        {
            options.OAuthClientId("coding_hub_microservices_swagger");
            options.DisplayRequestDuration();
            options.PathToSwaggerGenerator = "/swagger/docs";
            options.ReConfigureUpstreamSwaggerJson = AlterUpstream.AlterUpstreamSwaggerJson;
        }).UseOcelot().Wait();
    }
}