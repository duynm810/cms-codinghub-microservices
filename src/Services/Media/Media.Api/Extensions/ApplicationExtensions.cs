using Shared.Constants;

namespace Media.Api.Extensions;

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

        app.UseStaticFiles();

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DocumentTitle = $"{SwaggerConsts.MediaApi} Documentation";
            c.OAuthClientId("coding_hub_microservices_swagger");
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{SwaggerConsts.MediaApi} v1");
            c.DisplayOperationId(); // Show function name in swagger
            c.DisplayRequestDuration();
        });

        app.MapDefaultControllerRoute();
    }
}