using HealthChecks.UI.Client;
using Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Post.Application;
using Post.GRPC.Extensions;
using Post.GRPC.Services;
using Post.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Start {EnvironmentApplicationName} up", builder.Environment.ApplicationName);

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    
    // Config JSON files and environment variables
    builder.AddAppConfiguration();

    // Extracts configuration settings from appsettings.json and registers them with the service collection
    builder.Services.ConfigureSettings(configuration);

    // Configure health checks
    builder.Services.ConfigureHealthChecks();

    // Config application services in Post.Application
    builder.Services.AddApplicationServices();

    // Config infrastructure services in Post.Infrastructure
    builder.Services.AddInfrastructureServices(configuration);
    
    // Add services to the container.
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
    Log.Fatal(e, "Unhandled exception: {EMessage}", e.Message);
}
finally
{
    Log.Information("Shut down {EnvironmentApplicationName} down", builder.Environment.ApplicationName);
    Log.CloseAndFlush();
}