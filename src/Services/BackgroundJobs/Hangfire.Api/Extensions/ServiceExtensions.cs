using Contracts.Scheduled;
using Contracts.Services.Interfaces;
using Hangfire.Api.Services;
using Hangfire.Api.Services.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using Infrastructure.Scheduled;
using Infrastructure.Services;
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
        // Register configuration settings
        services.ConfigureSettings(configuration);
    
        // Register Hangfire services
        services.ConfigureHangfireServices();
    
        // Register MassTransit with RabbitMQ
        services.ConfigureMassTransitWithRabbitMq();

        // Register core services
        services.ConfigureCoreServices();

        // Register additional services
        services.ConfigureOtherServices();

        // Register Swagger services
        services.ConfigureSwaggerServices();

        // Register health checks
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
        
        var eventBusSetings = configuration.GetSection(nameof(EventBusSettings)).Get<EventBusSettings>() 
                              ?? throw new ArgumentNullException($"{nameof(EventBusSettings)} is not configured properly");

        services.AddSingleton(eventBusSetings);
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

    private static void ConfigureCoreServices(this IServiceCollection services)
    {
        services.AddTransient<IScheduledJobService, HangfireService>()
            .AddScoped<ISmtpEmailService, SmtpEmailService>()
            .AddScoped<IBackgroundJobService, BackgroundJobService>();
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