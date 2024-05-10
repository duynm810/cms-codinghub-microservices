using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Category.Api.Extensions;

public static class ApplicationExtensions
{
    /// <summary>
    /// Configures the HTTP request pipeline with essential middleware components such as Swagger UI, routing, and endpoint mapping.
    /// </summary>
    /// <param name="app">The IApplicationBuilder to configure the middleware pipeline.</param>
    public static void ConfigurePipeline(this IApplicationBuilder app)
    {
        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DocumentTitle = "Category Swagger UI";
            c.SwaggerEndpoint("CategoryAPI/swagger.json", "Category API");
            c.DisplayOperationId(); // Show function name in swagger
            c.DisplayRequestDuration();
        });

        // Enables routing in the application.
        app.UseRouting();

        //app.UseHttpsRedirection();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            endpoints.MapDefaultControllerRoute();
        });
    }
}