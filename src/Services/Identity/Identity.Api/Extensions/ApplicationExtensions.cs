using HealthChecks.UI.Client;
using IdentityServer4.AccessTokenValidation;
using Infrastructure.Extensions;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Shared.Constants;

namespace Identity.Api.Extensions;

public static class ApplicationExtensions
{
    public static void ConfigurePipeline(this WebApplication app)
    {
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
        
        if (app.Environment.IsDevelopment() || app.Environment.IsLocal())
        {
            app.UseDeveloperExceptionPage();
            
            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = $"{SwaggerConsts.IdentityApi} Documentation";
                c.OAuthClientId("coding_hub_microservices_swagger");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{SwaggerConsts.IdentityApi} v1");
                c.DisplayOperationId(); // Show function name in swagger
                c.DisplayRequestDuration();
            });
        }
        
        app.UseMiddleware<ErrorWrappingMiddleware>();

        app.UseSerilogRequestLogging();

        app.UseCors("CorsPolicy");

        // Uncomment if you want to add a UI
        app.UseStaticFiles();
        app.UseRouting();

        // Fix can't log in same site
        // Set cookie policy before authentication/authorization setup
        app.UseCookiePolicy();

        app.UseIdentityServer();

        app.UseAuthorization();

        app.UseAuthentication();

        app.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapDefaultControllerRoute().RequireAuthorization(IdentityServerAuthenticationDefaults.AuthenticationScheme);

        // Uncomment if you want to add a UI
        app.MapRazorPages().RequireAuthorization();
    }
}