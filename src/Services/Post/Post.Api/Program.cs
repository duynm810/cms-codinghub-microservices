using Logging;
using Post.Api.Extensions;
using Post.Application;
using Post.Domain.Interfaces;
using Post.Infrastructure;
using Serilog;
using Shared.Constants;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Initialize console logging for application startup
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting up {ApplicationName}", builder.Environment.ApplicationName);

try
{
    // Configure Serilog as the logging provider
    builder.Host.UseSerilog(Serilogger.Configure);

    // Load configuration from JSON files and environment variables
    builder.AddAppConfiguration();

    // Add configure settings get in appsettings
    builder.Services.ConfigureSettings(configuration);

    // Add health checks
    builder.Services.ConfigureHealthChecks();

    // Add application services in Post.Application
    builder.Services.AddApplicationServices();

    // Add infrastructure services in Post.Infrastructure
    builder.Services.AddInfrastructureServices(configuration);

    builder.Services.AddControllers();

    // Another services
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
        c.SwaggerDoc(SystemConsts.PostApi, new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "Post Api for Administrators",
            Description =
                "API for CMS core domain. This domain keeps track of campaigns, campaign rules, and campaign execution."
        });
    });

    var app = builder.Build();

    // Set up middleware and request handling pipeline
    app.ConfigurePipeline();

    // Seed database with initial data and start the application
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
    await seeder.InitialiseAsync();
    await seeder.SeedAsync();

    app.Run();
}
catch (Exception e)
{
    var type = e.GetType().Name;
    if (type.Equals("HostAbortedException", StringComparison.Ordinal)) throw;

    Log.Fatal(e, $"{ErrorMessageConsts.Common.UnhandledException}: {e.Message}");
}
finally
{
    // Ensure proper closure of application and flush logs
    Log.Information("Shutting down {ApplicationName} complete", builder.Environment.ApplicationName);
    Log.CloseAndFlush();
}