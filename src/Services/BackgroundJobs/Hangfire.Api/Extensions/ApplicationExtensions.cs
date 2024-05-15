using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Shared.Constants;

namespace Hangfire.Api.Extensions;

public static class ApplicationExtensions
{
    public static void ConfigurePipeline(this IApplicationBuilder app)
    {
        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DocumentTitle = "Hangfire Swagger UI";
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{SwaggerConsts.HangfireApi} v1");
            c.DisplayOperationId(); // Show function name in swagger
            c.DisplayRequestDuration();
        });

        // Enables routing in the application.
        app.UseRouting();

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