namespace Identity.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddInfrastructureService(this IServiceCollection services)
    {
        // Register identity server
        services.AddIdentityServer();
        
        // Register additional services
        services.AddAdditionalServices();
    }
    
    private static void AddIdentityServer(this IServiceCollection services)
    {
        services.AddIdentityServer(options =>
            {
                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients);
    }

    private static void AddAdditionalServices(this IServiceCollection services)
    {
        // uncomment if you want to add a UI
        services.AddRazorPages();
    }
}