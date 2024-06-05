using Infrastructure.Middlewares;
using Shared.Constants;

namespace Tag.Api.Extensions;

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

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DocumentTitle = $"{SwaggerConsts.TagApi} Documentation";
            c.OAuthClientId("coding_hub_microservices_swagger");
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{SwaggerConsts.TagApi} v1");
            c.DisplayOperationId(); // Show function name in swagger
            c.DisplayRequestDuration();
        });

        app.UseMiddleware<ErrorWrappingMiddleware>();

        // Enables routing in the application.
        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapDefaultControllerRoute();
    }
}