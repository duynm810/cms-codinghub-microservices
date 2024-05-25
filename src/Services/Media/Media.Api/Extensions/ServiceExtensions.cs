using Media.Api.Services;
using Media.Api.Services.Interfaces;
using Shared.Settings;

namespace Media.Api.Extensions;

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

        // Register repository and related services
        services.AddRepositoryAndDomainServices();

        // Register additional services
        services.AddAdditionalServices();

        // Register Swagger services
        services.AddSwaggerConfiguration();
    }

    private static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var mediaSettings = configuration.GetSection(nameof(MediaSettings)).Get<MediaSettings>()
                            ?? throw new ArgumentNullException(
                                $"{nameof(MediaSettings)} is not configured properly");

        services.AddSingleton(mediaSettings);
    }

    private static void AddRepositoryAndDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IMediaService, MediaService>();
    }

    private static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen();
    }

    private static void AddAdditionalServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    }
}