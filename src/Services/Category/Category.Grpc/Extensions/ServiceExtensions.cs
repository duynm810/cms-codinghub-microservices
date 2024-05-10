using Category.Grpc.Persistence;
using Category.Grpc.Repositories;
using Category.Grpc.Repositories.Interfaces;
using Category.Grpc.Services.BackgroundServices;
using Grpc.HealthCheck;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Shared.Configurations;

namespace Category.Grpc.Extensions;

public static class ServiceExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Extracts configuration settings from appsettings.json and registers them with the service collection
        services.ConfigureSettings(configuration);

        // Configures and registers the database context with the service collection
        services.ConfigureDbContext();

        // Configures and registers repository and services
        services.ConfigureRepositoryServices();

        // Configures health checks
        services.ConfigureHealthChecks();
    }

    private static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton(databaseSettings);
    }

    private static void ConfigureDbContext(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        var builder = new MySqlConnectionStringBuilder(databaseSettings.ConnectionString);
        services.AddDbContext<CategoryContext>(m => m.UseMySql(builder.ConnectionString,
            ServerVersion.AutoDetect(builder.ConnectionString), e =>
            {
                e.MigrationsAssembly("Category.Api");
                e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            }));
    }

    private static void ConfigureRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
    }

    private static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton<HealthServiceImpl>();
        services.AddHostedService<StatusService>();

        services.AddHealthChecks().AddMySql(connectionString: databaseSettings.ConnectionString,
                name: "Category MySQL Health",
                failureStatus: HealthStatus.Degraded)
            .AddCheck("Category gRPC Health", () => HealthCheckResult.Healthy());
    }
}