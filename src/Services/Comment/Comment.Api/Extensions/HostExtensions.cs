using Comment.Api.GrpcServices.Interfaces;
using Comment.Api.Persistence;
using MongoDB.Driver;
using Shared.Settings;
using ILogger = Serilog.ILogger;

namespace Comment.Api.Extensions;

public static class HostExtensions
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        var mongodbSettings = services.GetService<MongoDbSettings>() ??
                              throw new ArgumentNullException(
                                  $"{nameof(MongoDbSettings)} is not configured properly");

        var mongoClient = services.GetRequiredService<IMongoClient>();
        var postGrpcService = services.GetRequiredService<IPostGrpcService>();
        var logger = services.GetRequiredService<ILogger>();
        
        new CommentSeedData(postGrpcService, logger).SeedDataAsync(mongoClient, mongodbSettings).Wait();

        return host;
    }
}