using System.Security.Authentication;
using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using Shared.Configurations;

namespace Infrastructure.Extensions;

public static class HangfireExtensions
{
    public static void AddHangfireServices(this IServiceCollection services)
    {
        var hangfireSettings = services.GetOptions<HangfireSettings>(nameof(HangfireSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(HangfireSettings)} is not configured properly");

        services.ConfigureHangfire(hangfireSettings);
        services.AddHangfireServer(serverOptions =>
        {
            serverOptions.ServerName = hangfireSettings.ServerName;
        });
    }

    private static void ConfigureHangfire(this IServiceCollection services,
        HangfireSettings hangfireSettings)
    {
        if (string.IsNullOrEmpty(hangfireSettings.Storage.DbProvider))
            throw new Exception($"{nameof(hangfireSettings)} is not configured properly");

        switch (hangfireSettings.Storage.DbProvider.ToLower())
        {
            case "mongodb":
                var mongoUrlBuilder = new MongoUrlBuilder(hangfireSettings.Storage.ConnectionString);

                var mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(hangfireSettings.Storage.ConnectionString));
                
                mongoClientSettings.SslSettings = new SslSettings
                {
                    EnabledSslProtocols = SslProtocols.Tls12
                };
                
                var mongoClient = new MongoClient(mongoClientSettings);

                var mongoStorageOptions = new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy()
                    },
                    CheckConnection = true,
                    Prefix = "SchedulerQueue",
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
                };
                
                services.AddHangfire((provider, config) =>
                {
                    config.UseSimpleAssemblyNameTypeSerializer()
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseRecommendedSerializerSettings()
                        .UseConsole()
                        .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, mongoStorageOptions);

                    var jsonSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };
                    
                    config.UseSerializerSettings(jsonSettings);
                });
                
                services.AddHangfireConsoleExtensions();
                break;

            case "postgresql":
                break;

            case "mssql":
                break;

            default:
                throw new Exception(
                    $"Hangfire Storage Provider {hangfireSettings.Storage.DbProvider} is not supported.");
        }
    }
}