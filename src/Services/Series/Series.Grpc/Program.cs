using Logging;
using Series.Grpc.Extensions;
using Serilog;
using Shared.Constants;

var builder = WebApplication.CreateBuilder(args);

// Initialize console logging for application startup
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting up {EnvironmentApplicationName}", builder.Environment.ApplicationName);

try
{
    // Configure Serilog as the logging provider
    builder.Host.UseSerilog(Serilogger.Configure);

    // Load configuration from JSON files and environment variables
    builder.AddAppConfiguration();

    // Register application infrastructure services
    builder.Services.AddInfrastructureServices(builder.Configuration);

    var app = builder.Build();

    // Set up middleware and request handling pipeline
    app.ConfigurePipeline();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, $"{ErrorMessageConsts.Common.UnhandledException}: {e.Message}");
}
finally
{
    // Ensure proper closure of application and flush logs
    Log.Information("Shutting down {ApplicationName} complete", builder.Environment.ApplicationName);
    Log.CloseAndFlush();
}