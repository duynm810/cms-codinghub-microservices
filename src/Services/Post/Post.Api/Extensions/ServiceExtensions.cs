using Infrastructure.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Configurations;
using Shared.Settings;

namespace Post.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton(databaseSettings);

        var grpcSettings = configuration.GetSection(nameof(GrpcSettings)).Get<GrpcSettings>()
                           ?? throw new ArgumentNullException($"{nameof(GrpcSettings)} is not configured properly");

        services.AddSingleton(grpcSettings);

        var eventBusSetings = configuration.GetSection(nameof(EventBusSettings)).Get<EventBusSettings>()
                              ?? throw new ArgumentNullException(
                                  $"{nameof(EventBusSettings)} is not configured properly");

        services.AddSingleton(eventBusSetings);

        var emailTemplateSettings = configuration.GetSection(nameof(EmailTemplateSettings)).Get<EmailTemplateSettings>()
                                    ?? throw new ArgumentNullException(
                                        $"{nameof(EmailTemplateSettings)} is not configured properly");

        services.AddSingleton(emailTemplateSettings);

        var apiConfigurations = configuration.GetSection(nameof(ApiConfigurations)).Get<ApiConfigurations>()
                                ?? throw new ArgumentNullException(
                                    $"{nameof(ApiConfigurations)} is not configured properly");

        services.AddSingleton(apiConfigurations);
    }

    public static void AddHealthCheckServices(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        var cacheSettings = services.GetOptions<CacheSettings>(nameof(CacheSettings)) ??
                            throw new ArgumentNullException(
                                $"{nameof(CacheSettings)} is not configured properly");
        
        var elasticsearchConfigurations = services.GetOptions<ElasticConfigurations>(nameof(ElasticConfigurations)) ??
                                          throw new ArgumentNullException(
                                              $"{nameof(ElasticConfigurations)} is not configured properly");

        services.AddHealthChecks()
            .AddNpgSql(databaseSettings.ConnectionString,
                name: "PostgreSQL Health",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "db", "postgre" })
            .AddRedis(cacheSettings.ConnectionString,
                name: "Redis Health",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "cache", "redis" })
            .AddElasticsearch(
                elasticsearchConfigurations.Uri,
                name: "Elasticsearch Health",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "search", "elasticsearch" });
    }
}