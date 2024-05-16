using Infrastructure.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Configurations;

namespace Post.Api.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton(databaseSettings);
        
        var grpcSettings = configuration.GetSection(nameof(GrpcSettings)).Get<GrpcSettings>() 
                           ?? throw new ArgumentNullException($"{nameof(GrpcSettings)} is not configured properly");
        
        services.AddSingleton(grpcSettings);
        
        var eventBusSetings = configuration.GetSection(nameof(EventBusSettings)).Get<EventBusSettings>() 
                           ?? throw new ArgumentNullException($"{nameof(EventBusSettings)} is not configured properly");

        services.AddSingleton(eventBusSetings);
    }

    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddHealthChecks()
            .AddNpgSql(databaseSettings.ConnectionString,
                name: "PostgreSQL Health",
                failureStatus: HealthStatus.Degraded);
    }
}