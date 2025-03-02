using Grpc.HealthCheck;
using Infrastructure.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Post.Grpc.Services.BackgroundServices;
using Shared.Configurations;
using Shared.Settings;

namespace Post.Grpc.Extensions;

public static class ServiceExtensions
{
    public static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    }

    public static void AddHealthCheckServices(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");
        
        services.AddSingleton<HealthServiceImpl>();
        services.AddHostedService<StatusService>();

        services.AddGrpcHealthChecks()
            .AddNpgSql(databaseSettings.ConnectionString,
                name: "PostgreSQL Health",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "db", "postgre" })
            .AddCheck("gRPC Health", 
                () => HealthCheckResult.Healthy(), 
                new[] { "grpc" });
    }
}