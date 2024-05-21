using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Shared.Constants;

namespace Identity.Api.Extensions;

public static class ApplicationExtensions
{
    public static void ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DocumentTitle = $"{SwaggerConsts.IdentityApi} Documentation";
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{SwaggerConsts.IdentityApi} v1");
            c.DisplayOperationId(); // Show function name in swagger
            c.DisplayRequestDuration();
        });

        app.UseSerilogRequestLogging();
        
        app.UseCors("CorsPolicy");

        // Uncomment if you want to add a UI
        app.UseStaticFiles();
        app.UseRouting();
        
        // Fix can't log in same site
        // Set cookie policy before authentication/authorization setup
        app.UseCookiePolicy();

        app.UseIdentityServer();

        // Uncomment if you want to add a UI
        app.UseAuthorization();
        
        app.UseAuthentication();
        
        app.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        
        app.MapControllers();
        
        app.MapRazorPages().RequireAuthorization();
    }
}