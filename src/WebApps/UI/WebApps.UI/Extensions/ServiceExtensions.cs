using Contracts.Commons.Interfaces;
using Infrastructure.Commons;
using Shared.Settings;
using WebApps.UI.Services;
using WebApps.UI.Services.Interfaces;

namespace WebApps.UI.Extensions;

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
        
        // Register repository services
        services.AddRepositoryAndDomainServices();
        
        // Register http client services
        services.AddHttpClientServices();

        // Register api client services
        services.AddApiClientServices();

        // Register additional services
        services.AddAdditionalServices();

        // Register razor pages runtime (using for dev)
        services.AddRazorPagesRuntimeConfiguration();
    }

    private static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var apiSettings = configuration.GetSection(nameof(ApiSettings)).Get<ApiSettings>()
                          ?? throw new ArgumentNullException(
                              $"{nameof(ApiSettings)} is not configured properly");

        services.AddSingleton(apiSettings);
    }

    private static void AddApiClientServices(this IServiceCollection services)
    {
        services
            .AddScoped<IBaseApiClient, BaseApiClient>()
            .AddScoped<ICategoryApiClient, CategoryApiClient>();
    }
    
    private static void AddRepositoryAndDomainServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ISerializeService, SerializeService>();
    }

    private static void AddAdditionalServices(this IServiceCollection services)
    {
        services.AddControllersWithViews();
    }

    private static void AddRazorPagesRuntimeConfiguration(this IServiceCollection services)
    {
        var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (enviroment == Environments.Development)
        {
            services.AddRazorPages().AddRazorRuntimeCompilation();
        }
    }

    private static void AddHttpClientServices(this IServiceCollection services)
    {
        services.AddHttpClient();
    }
}