using HealthChecks.UI.Client;
using Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Post.Application;
using Post.Grpc.Extensions;
using Post.Grpc.Services;
using Post.Infrastructure;
using Serilog;
using Shared.Constants;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Initialize console logging for application startup
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting up {EnvironmentApplicationName}", builder.Environment.ApplicationName);

try
{
    // Configure Serilog as the logging provider
    builder.Host.UseSerilog(Serilogger.Configure);

    // Load configuration from JSON files and environment variables
    builder.AddAppConfiguration();

    // Add health checks to checks database
    builder.Services.AddHealthCheckServices();

    // Add application services in Post.Application
    builder.Services.AddApplicationServices();

    // Add infrastructure services in Post.Infrastructure
    builder.Services.AddInfrastructureServices(configuration);

    // Add grpc services
    builder.Services.AddGrpc();
    builder.Services.AddGrpcReflection();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.MapGrpcService<PostService>();
    app.MapGet("/",
        () =>
            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

    app.MapHealthChecks("/hc", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapGrpcHealthChecksService();

    app.MapGrpcReflectionService();

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