namespace WebApps.HealthCheck.Extensions;

public static class ServiceExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register additional services
        services.AddAdditionalServices();
        
        // Register health checks
        services.AddHealthCheckServices();
    }
    
    private static void AddAdditionalServices(this IServiceCollection services)
    {
        services.AddControllersWithViews();
    }

    private static void AddHealthCheckServices(this IServiceCollection services)
    {
        services.AddHealthChecksUI().AddInMemoryStorage();
    }
}