using Category.GRPC.Persistence;
using Category.GRPC.Repositories;
using Category.GRPC.Repositories.Interfaces;
using Category.GRPC.Services.BackgroundServices;
using Grpc.HealthCheck;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Shared.Configurations;

namespace Category.GRPC.Extensions;

public static class ServiceExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Extracts configuration settings from appsettings.json and registers them with the service collection
        services.ConfigureSettings(configuration);

        // Configures and registers the database context with the service collection
        services.ConfigureDbContext(configuration);

        // Configures and registers repository and services
        services.ConfigureRepositoryServices();

        // Configures health checks
        services.ConfigureHealthChecks(configuration);
    }

    private static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton(databaseSettings);
    }

    private static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        if (databaseSettings == null || string.IsNullOrEmpty(databaseSettings.ConnectionString))
        {
            throw new ArgumentNullException($"{nameof(DatabaseSettings)} ConnectionString is not configured properly");
        }

        var builder = new MySqlConnectionStringBuilder(databaseSettings.ConnectionString);
        services.AddDbContext<CategoryContext>(m => m.UseMySql(builder.ConnectionString,
            ServerVersion.AutoDetect(builder.ConnectionString), e =>
            {
                e.MigrationsAssembly("Category.API");
                e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            }));
    }

    private static void ConfigureRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
    }

    private static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton<HealthServiceImpl>();
        services.AddHostedService<StatusService>();

        services.AddHealthChecks().AddMySql(connectionString: databaseSettings.ConnectionString,
                name: "Category MySQL Health",
                failureStatus: HealthStatus.Degraded)
            .AddCheck("Category gRPC Health", () => HealthCheckResult.Healthy());
    }
}