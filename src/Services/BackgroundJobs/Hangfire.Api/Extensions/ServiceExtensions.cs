using Infrastructure.Configurations;
using Infrastructure.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Configurations;
using Shared.Constants;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Hangfire.Api.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Registers various infrastructure services including Swagger and other essential services.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The configuration to be used by the services.</param>
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Extracts configuration settings from appsettings.json and registers them with the service collection
        services.ConfigureSettings(configuration);

        // Configures and registers repository and services
        services.ConfigureRepositoryServices();

        // Configures and registers essential services
        services.ConfigureOtherServices();

        // Configures swagger services
        services.ConfigureSwaggerServices();
        
        // Configure health checks
        services.ConfigureHealthChecks();
    }

    private static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var hangfireSettings = configuration.GetSection(nameof(HangfireSettings)).Get<HangfireSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(HangfireSettings)} is not configured properly");

        services.AddSingleton(hangfireSettings);

        var smtpEmailSettings = configuration.GetSection(nameof(SmtpEmailSettings)).Get<SmtpEmailSettings>()
                                ?? throw new ArgumentNullException(
                                    $"{nameof(SmtpEmailSettings)} is not configured properly");

        services.AddSingleton(smtpEmailSettings);
    }

    private static void ConfigureSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Version = "v1",
                Title = $"{SwaggerConsts.HangfireApi} for Administrators",
                Description =
                    "API for CMS core domain. This domain keeps track of campaigns, campaign rules, and campaign execution."
            });
        });
    }

    private static void ConfigureRepositoryServices(this IServiceCollection services)
    {
    }

    private static void ConfigureOtherServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    }

    private static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var hangfireSettings = services.GetOptions<HangfireSettings>(nameof(HangfireSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(HangfireSettings)} is not configured properly");

        services.AddHealthChecks().AddMongoDb(hangfireSettings.Storage.ConnectionString,
            name: "MongoDb Health",
            failureStatus: HealthStatus.Degraded);
    }
}