using Ocelot.Gw.Configs;
using Ocelot.Middleware;

namespace Ocelot.Gw.Extensions;

public static class ApplicationExtensions
{
    /// <summary>
    /// Configures the HTTP request pipeline with essential middleware components such as Swagger UI, routing, and endpoint mapping.
    /// </summary>
    /// <param name="app">The IApplicationBuilder to configure the middleware pipeline.</param>
    public static void ConfigurePipeline(this IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseCors("CorsPolicy");

        app.UseSwagger();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseSwaggerForOcelotUI(options =>
        {
            options.PathToSwaggerGenerator = "/swagger/docs";
            options.ReConfigureUpstreamSwaggerJson = AlterUpstream.AlterUpstreamSwaggerJson;
        }).UseOcelot().Wait();
    }
}