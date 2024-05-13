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
        // Configures and registers ocelot services
        services.ConfigureOcelot(configuration);

        // Configures and registers Cors Origin (CORS) services
        services.ConfigureCorsOrigin(configuration);
        
        // Configures swagger services
        services.ConfigureSwaggerServices();
        
        // Configures and registers essential services
        services.ConfigureOtherServices();
    }

    private static void ConfigureOcelot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOcelot(configuration)
            .AddPolly()
            .AddCacheManager(x => x.WithDictionaryHandle());
    }

    private static void ConfigureCorsOrigin(this IServiceCollection services, IConfiguration configuration)
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
    }

    private static void ConfigureSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen();
    }
}