using Grpc.HealthCheck;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PostInTag.Grpc.Persistence;
using PostInTag.Grpc.Repositories;
using PostInTag.Grpc.Repositories.Interfaces;
using PostInTag.Grpc.Services.BackgroundServices;
using Shared.Configurations;
using Shared.Settings;

namespace PostInTag.Grpc.Extensions;

public static class ServiceExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register app configuration settings
        services.AddConfigurationSettings(configuration);

        // Register database context
        services.AddDatabaseContext();

        // Register gRPC services
        services.AddGrpcServices();

        // Register core services
        services.AddCoreInfrastructure();

        // Register repository services
        services.AddRepositoryAndDomainServices();

        // Register health checks
        services.AddHealthCheckServices();
    }

    private static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton(databaseSettings);
    }

    private static void AddDatabaseContext(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddDbContextPool<PostInTagContext>(opts =>
        {
            opts.UseNpgsql(databaseSettings.ConnectionString, optionsBuilder =>
            {
                optionsBuilder.UseNodaTime();
                optionsBuilder.MigrationsAssembly(typeof(PostInTagContext).Assembly.FullName);
                optionsBuilder.EnableRetryOnFailure();
                optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior
                    .SplitQuery); // If query have multiple include entities, using split query separate SQL query
            });
            opts.UseSnakeCaseNamingConvention();
        });
    }

    private static void AddGrpcServices(this IServiceCollection services)
    {
        services.AddGrpc();
        services.AddGrpcReflection();
    }

    private static void AddRepositoryAndDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IPostInTagRepository, PostInTagRepository>();
    }

    private static void AddHealthCheckServices(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        var elasticsearchConfigurations = services.GetOptions<ElasticConfigurations>(nameof(ElasticConfigurations)) ??
                                          throw new ArgumentNullException(
                                              $"{nameof(ElasticConfigurations)} is not configured properly");

        services.AddSingleton<HealthServiceImpl>();
        services.AddHostedService<StatusService>();

        services.AddGrpcHealthChecks()
            .AddNpgSql(connectionString: databaseSettings.ConnectionString,
                name: "PostgreSQL Health",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "db", "postgre" })
            .AddCheck("gRPC Health",
                () => HealthCheckResult.Healthy(),
                new[] { "grpc" })
            .AddElasticsearch(
                elasticsearchConfigurations.Uri,
                name: "Elasticsearch Health",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "search", "elasticsearch" });
    }
}