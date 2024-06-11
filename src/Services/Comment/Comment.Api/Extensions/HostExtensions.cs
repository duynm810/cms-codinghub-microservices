using Comment.Api.Persistence;
using MongoDB.Driver;
using Shared.Settings;

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
        new CommentSeedData().SeedDataAsync(mongoClient, mongodbSettings).Wait();

        return host;
    }
}