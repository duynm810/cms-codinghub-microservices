using Infrastructure.Identity;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Gw.Aggregators;
using Ocelot.Provider.Polly;
using Shared.Configurations;

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
        // Register app configuration settings
        services.AddConfigurationSettings(configuration);

        // Register Ocelot services
        services.AddOcelotConfiguration(configuration);

        // Register Ocelot with Swagger support
        services.AddOcelotSwaggerConfiguration(configuration);

        // Register CORS services
        services.AddCorsConfiguration(configuration);

        // Register additional necessary services
        services.AddAdditionalServices();

        // Register authentication services
        services.AddAuthenticationServices();
    }

    private static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var apiConfigurations = configuration.GetSection(nameof(ApiConfigurations)).Get<ApiConfigurations>()
                                ?? throw new ArgumentNullException(
                                    $"{nameof(ApiConfigurations)} is not configured properly");

        services.AddSingleton(apiConfigurations);
    }

    private static void AddOcelotConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOcelot(configuration)
            .AddSingletonDefinedAggregator<DashboardAggregator>()
            .AddSingletonDefinedAggregator<FooterAggregator>()
            .AddSingletonDefinedAggregator<SidebarAggregator>()
            .AddPolly()
            .AddCacheManager(x => x.WithDictionaryHandle());
    }

    private static void AddOcelotSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerForOcelot(configuration); // Add this line to register SwaggerForOcelot services
    }

    private static void AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration["AllowOrigins"];

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                if (!string.IsNullOrEmpty(origins))
                {
                    builder.WithOrigins(origins.Split(','))
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
                else
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            });
        });
    }

    private static void AddAdditionalServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
}