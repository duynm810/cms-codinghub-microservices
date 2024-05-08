using Infrastructure.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Configurations;

namespace Post.API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton(databaseSettings);
    }

    public static void ConfigureHealthChecks(this IServiceCollection services,
        IConfiguration configuration)
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