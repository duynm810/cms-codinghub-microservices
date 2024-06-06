using Category.Grpc.Extensions;
using Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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

    builder.WebHost.ConfigureKestrel(options =>
    {
        if (builder.Environment.IsDevelopment())
        {
            options.ListenAnyIP(5004, listenOptions => listenOptions.Protocols = HttpProtocols.Http1AndHttp2);
        }
    });

    var app = builder.Build();

    // Set up middleware and request handling pipeline
    app.ConfigurePipeline();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, $"{ErrorMessagesConsts.Common.UnhandledException}: {e.Message}");
}
finally
{
    // Ensure proper closure of application and flush logs
    Log.Information("Shutting down {ApplicationName} complete", builder.Environment.ApplicationName);
    Log.CloseAndFlush();
}