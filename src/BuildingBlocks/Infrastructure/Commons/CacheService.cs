using Contracts.Commons.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Commons;

public class CacheService(IDistributedCache distributedCache, ISerializeService serializeService) : ICacheService
{
    public async Task<T?> GetAsync<T>(string cacheKey)
    {
        var cachedData = await distributedCache.GetStringAsync(cacheKey);
        return !string.IsNullOrEmpty(cachedData) ? serializeService.Deserialize<T>(cachedData) : default;
    }

    public async Task SetAsync<T>(string cacheKey, T data, TimeSpan? expiration = null)
    {
        var serializedData = serializeService.Serialize(data);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5) // Default cache time
        };
        await distributedCache.SetStringAsync(cacheKey, serializedData, options);
    }

    public async Task RemoveAsync(string cacheKey)
    {
        await distributedCache.RemoveAsync(cacheKey);
    }

    public async Task RemoveMultipleAsync(IEnumerable<string> cacheKeys)
    {
        foreach (var cacheKey in cacheKeys)
        {
            await distributedCache.RemoveAsync(cacheKey);
        }
    }
}