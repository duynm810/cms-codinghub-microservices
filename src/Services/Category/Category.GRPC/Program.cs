using Category.GRPC.Extensions;
using Category.GRPC.Services;
using Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Start {EnvironmentApplicationName} up", builder.Environment.ApplicationName);

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    
    // Config JSON files and environment variables
    builder.AddAppConfiguration();

    // Register services
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // Add services to the container.
    builder.Services.AddGrpc();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.MapGrpcService<CategoryService>();
    app.MapGet("/",
        () =>
            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

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