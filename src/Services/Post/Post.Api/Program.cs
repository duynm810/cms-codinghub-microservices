using Infrastructure.Extensions;
using Infrastructure.Identity;
using Logging;
using Post.Api.Extensions;
using Post.Application;
using Post.Domain.Interfaces;
using Post.Infrastructure;
using Serilog;
using Shared.Constants;

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

    // Register app configuration settings
    builder.Services.AddConfigurationSettings(configuration);
    
    builder.Services.AddMassTransitWithRabbitMq();
    
    // Register health checks
    builder.Services.AddHealthCheckServices();

    // Register application services
    builder.Services.AddApplicationServices();

    // Register infrastructure
    builder.Services.AddInfrastructureServices(configuration);

    builder.Services.AddControllers();

    // Another services
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

    // Register authentication services
    builder.Services.AddAuthenticationServices();

    // Register authorization services
    builder.Services.AddAuthorizationServices();
    
    // Register Swagger services
    builder.Services.AddSwaggerConfiguration();

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