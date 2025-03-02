using HealthChecks.UI.Client;
using Infrastructure.Extensions;
using Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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

    // Register AutoMapper
    builder.Services.AddAutoMapperConfiguration();

    // Add health checks to checks database
    builder.Services.AddHealthCheckServices();

    // Add application services in Post.Application
    builder.Services.AddApplicationServices();

    // Add infrastructure services in Post.Infrastructure
    builder.Services.AddInfrastructureServices(configuration);

    // Add grpc services
    builder.Services.AddGrpc();
    builder.Services.AddGrpcReflection();
    
    // Set up port and protocol that the Kestrel server listens on to receive requests to the application
    // Thiết lập cổng và giao thức mà máy chủ Kestrel lắng nghe để tiếp nhận các yêu cầu đến ứng dụng
    builder.WebHost.ConfigureKestrel(options =>
    {
        // Config development environment (Cấu hình môi trường phát triển)
        if (builder.Environment.IsDevelopment())
        {
            options.ListenAnyIP(5103, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
        }
        
        // Config local(docker) environment (Cấu hình môi trường docker)
        if (builder.Environment.IsLocal() || builder.Environment.IsStaging())
        {
            // Configure health checks to use port 80 with HTTP/1. (Cấu hình health checks sử dụng cổng 80 với HTTP/1.)
            options.ListenAnyIP(80, listenOptions => listenOptions.Protocols = HttpProtocols.Http1);
            
            // Configure http client to use with HTTP/2 (Cấu hình HttpClient để gửi yêu cầu gRPC sử dụng HTTP/2.)
            options.ListenAnyIP(6103, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
        }
    });

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