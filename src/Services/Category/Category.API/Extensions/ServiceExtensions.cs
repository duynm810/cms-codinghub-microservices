using Category.API.Persistence;
using Category.API.Repositories;
using Category.API.Repositories.Interfaces;
using Category.API.Services;
using Category.API.Services.Interfaces;
using Contracts.Domains.Repositories;
using Infrastructure.Domains;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Shared.Configurations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Category.API.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Registers various infrastructure services including Swagger and other essential services.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The configuration to be used by the services.</param>
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Extracts configuration settings from appsettings.json and registers them with the service collection
        services.ConfigureDatabaseSettings(configuration);

        // Configures and registers the database context with the service collection
        services.ConfigureDbContext(configuration);

        // Configures and registers core services
        services.ConfigureCoreServices();

        // Configures and registers repository and services
        services.ConfigureRepositoryServices();

        // Configures and registers essential services
        services.ConfigureOtherServices();

        // Configures and registers AutoMapper
        services.ConfigureAutoMapper();

        // Configures swagger services
        services.ConfigureSwaggerServices();
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

    private static void ConfigureDatabaseSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(
                                   $"{nameof(DatabaseSettings)} is not configured properly");

        services.AddSingleton(databaseSettings);
    }

    private static void ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    }

    private static void ConfigureSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
            c.SwaggerDoc("CategoryAPI", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Version = "v1",
                Title = "Category API for Administrators",
                Description =
                    "API for CMS core domain. This domain keeps track of campaigns, campaign rules, and campaign execution."
            });
        });
    }

    private static void ConfigureCoreServices(this IServiceCollection services)
    {
        services
            .AddScoped(typeof(IRepositoryQueryBase<,,>), typeof(RepositoryQueryBase<,,>))
            .AddScoped(typeof(IRepositoryCommandBase<,,>), typeof(RepositoryCommandBase<,,>))
            .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
    }

    private static void ConfigureRepositoryServices(this IServiceCollection services)
    {
        services
            .AddScoped<ICategoryRepository, CategoryRepository>()
            .AddScoped<ICategoryService, CategoryService>();
    }

    private static void ConfigureOtherServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddEndpointsApiExplorer();
    }
}