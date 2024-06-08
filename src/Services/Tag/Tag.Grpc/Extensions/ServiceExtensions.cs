using Grpc.HealthCheck;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Shared.Settings;
using Tag.Grpc.Persistence;
using Tag.Grpc.Repositories;
using Tag.Grpc.Repositories.Interfaces;
using Tag.Grpc.Services.BackgroundServices;

namespace Tag.Grpc.Extensions;

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

        // Register repository services
        services.AddRepositoryAndDomainServices();

        // Register AutoMapper
        services.AddAutoMapperConfiguration();

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

        var builder = new MySqlConnectionStringBuilder(databaseSettings.ConnectionString);
        services.AddDbContext<TagContext>(m => m.UseMySql(builder.ConnectionString,
            ServerVersion.AutoDetect(builder.ConnectionString), e =>
            {
                e.MigrationsAssembly("Tag.Api");
                e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            }));
    }

    private static void AddGrpcServices(this IServiceCollection services)
    {
        services.AddGrpc();
        services.AddGrpcReflection();
    }

    private static void AddRepositoryAndDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ITagRepository, TagRepository>();
    }

    private static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    }

    private static void AddHealthCheckServices(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton<HealthServiceImpl>();
        services.AddHostedService<StatusService>();

        services.AddGrpcHealthChecks().AddMySql(connectionString: databaseSettings.ConnectionString,
                name: "MySQL Health",
                failureStatus: HealthStatus.Degraded)
            .AddCheck("gRPC Health", () => HealthCheckResult.Healthy());
    }
}