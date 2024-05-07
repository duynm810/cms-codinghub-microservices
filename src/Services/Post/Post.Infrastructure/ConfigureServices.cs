using Contracts.Domains.Repositories;
using Infrastructure.Domains;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Post.Domain.Interfaces;
using Post.Infrastructure.Persistence;
using Post.Infrastructure.Repositories;
using Shared.Configurations;

namespace Post.Infrastructure;

public static class ConfigureServices
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configures and registers the database context with the service collection
        services.ConfigureDbContext(configuration);

        // Configures and registers to seed post data
        services.ConfigureSeedData();

        // Configures and registers core services
        services.ConfigureCoreServices();

        // Configures and registers repository and services
        services.ConfigureRepositoryServices();
    }

    private static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        if (databaseSettings == null || string.IsNullOrEmpty(databaseSettings.ConnectionString))
        {
            throw new ArgumentNullException(
                $"{nameof(DatabaseSettings)} is not configured properly");
        }

        services.AddDbContext<PostContext>(options =>
        {
            options.UseSqlServer(databaseSettings.ConnectionString,
                builder =>
                    builder.MigrationsAssembly(typeof(PostContext).Assembly.FullName));
        });
    }

    private static void ConfigureSeedData(this IServiceCollection services)
    {
        services.AddScoped<IDatabaseSeeder, PostSeedData>();
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
        services.AddScoped<IPostRepository, PostRepository>();
    }
}