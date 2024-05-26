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
        // Register additional services
        services.AddAdditionalServices();
    }

    private static void AddAdditionalServices(this IServiceCollection services)
    {
        services.AddControllersWithViews();
    }
}