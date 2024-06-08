using Infrastructure.Extensions;
using Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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

    // Set up port and protocol that the Kestrel server listens on to receive requests to the application
    // Thiết lập cổng và giao thức mà máy chủ Kestrel lắng nghe để tiếp nhận các yêu cầu đến ứng dụng
    builder.WebHost.ConfigureKestrel(options =>
    {
        // Config development environment (Cấu hình môi trường phát triển)
        if (builder.Environment.IsDevelopment())
        {
            options.ListenAnyIP(5008, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
        }
        
        // Config local(docker) environment (Cấu hình môi trường docker)
        if (builder.Environment.IsEnvironment(EnvironmentConsts.Local))
        {
            // Configure health checks to use port 80 with HTTP/1. (Cấu hình health checks sử dụng cổng 80 với HTTP/1.)
            options.ListenAnyIP(80, listenOptions => listenOptions.Protocols = HttpProtocols.Http1);
            
            // Configure http client to use with HTTP/2 (Cấu hình HttpClient để gửi yêu cầu gRPC sử dụng HTTP/2.)
            options.ListenAnyIP(6008, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
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