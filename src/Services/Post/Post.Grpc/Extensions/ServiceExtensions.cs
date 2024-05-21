using Grpc.HealthCheck;
using Infrastructure.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Post.Grpc.Services.BackgroundServices;
using Shared.Settings;

namespace Post.Grpc.Extensions;

public static class ServiceExtensions
{
    public static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton(databaseSettings);
    }

    public static void AddHealthCheckServices(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton<HealthServiceImpl>();
        services.AddHostedService<StatusService>();

        services.AddHealthChecks()
            .AddNpgSql(databaseSettings.ConnectionString,
                name: "PostgreSQL Health",
                failureStatus: HealthStatus.Degraded)
            .AddCheck("Post gRPC Health", () => HealthCheckResult.Healthy());
    }
}