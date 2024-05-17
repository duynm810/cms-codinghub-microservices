using Microsoft.Extensions.Diagnostics.HealthChecks;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Polly;

namespace Ocelot.Gw.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Registers various infrastructure services including Swagger and other essential services.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The configuration to be used by the services.</param>
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Ocelot services
        services.ConfigureOcelot(configuration);

        // Register Ocelot with Swagger support
        services.ConfigureOcelotSwaggerServices(configuration);

        // Register CORS services
        services.ConfigureCorsServices(configuration);

        // Register additional necessary services
        services.ConfigureOtherServices();
    }

    private static void ConfigureOcelot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOcelot(configuration)
            .AddPolly()
            .AddCacheManager(x => x.WithDictionaryHandle());
    }

    private static void ConfigureOcelotSwaggerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerForOcelot(configuration); // Add this line to register SwaggerForOcelot services
    }

    private static void ConfigureCorsServices(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration["AllowOrigins"];

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", buider =>
            {
                if (origins != null)
                    buider.WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
            });
        });
    }

    private static void ConfigureOtherServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
}