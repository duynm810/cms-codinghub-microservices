using Microsoft.Extensions.DependencyInjection;
using Shared.Settings;

namespace Infrastructure.Extensions;

public static class RedisExtensions
{
    public static void AddRedisConfiguration(this IServiceCollection services)
    {
        var cacheSettings = services.GetOptions<CacheSettings>(nameof(CacheSettings)) ??
                            throw new ArgumentNullException($"{nameof(CacheSettings)} is not configured properly");

        //Redis Configuration
        services.AddStackExchangeRedisCache(options => { options.Configuration = cacheSettings.ConnectionString; });
    }
}