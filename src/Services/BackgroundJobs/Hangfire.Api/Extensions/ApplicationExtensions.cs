using Hangfire.Api.Filters;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Shared.Constants;
using Shared.Settings;

namespace Hangfire.Api.Extensions;

public static class ApplicationExtensions
{
    public static void ConfigurePipeline(this WebApplication app, IConfiguration configuration)
    {
        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DocumentTitle = $"{SwaggerConsts.HangfireApi} Documentation";
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{SwaggerConsts.HangfireApi} v1");
            c.DisplayOperationId(); // Show function name in swagger
            c.DisplayRequestDuration();
        });

        // Enables routing in the application.
        app.UseRouting();

        app.UseHangfireDashboard(configuration);

        app.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapDefaultControllerRoute();
    }

    private static void UseHangfireDashboard(this WebApplication app, IConfiguration configuration)
    {
        var configDashboard = configuration.GetSection("HangfireSettings:Dashboard").Get<DashboardOptions>()
                              ?? throw new ArgumentNullException(
                                  $"{nameof(HangfireSettings)} Dashboard is not configured properly");

        var hangfireSettings = configuration.GetSection("HangfireSettings").Get<HangfireSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(HangfireSettings)} is not configured properly");

        var hangfireRoute = hangfireSettings.Route;

        app.UseHangfireDashboard(hangfireRoute, new DashboardOptions
        {
            Authorization = new[] { new AuthorizationFilter() },
            DashboardTitle = configDashboard.DashboardTitle,
            StatsPollingInterval = configDashboard.StatsPollingInterval,
            AppPath = configDashboard.AppPath,
            IgnoreAntiforgeryToken = true
        });
    }
}