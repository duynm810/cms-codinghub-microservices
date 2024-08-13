using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using WebApps.UI.Routes;

namespace WebApps.UI.Extensions;

public static class ApplicationExtensions
{
    public static void ConfigurePipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/HttpError");
            app.UseHsts();
        }

        if (app.Environment.IsProduction() || app.Environment.IsStaging())
        {
            app.UseHttpsRedirection();
            
            var forwardedHeaderOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            
            forwardedHeaderOptions.KnownNetworks.Clear(); // Để tránh các mạng nội bộ
            forwardedHeaderOptions.KnownProxies.Clear();  // Để tránh các proxy cụ thể

            app.UseForwardedHeaders(forwardedHeaderOptions);
        }

        app.UseStatusCodePagesWithReExecute("/HttpError/{0}");

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();
        
        app.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // Register routes
        RouteMap.RegisterRoutes(app);
    }
}