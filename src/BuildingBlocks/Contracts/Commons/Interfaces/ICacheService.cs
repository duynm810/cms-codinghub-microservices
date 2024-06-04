namespace Contracts.Commons.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string cacheKey);

    Task SetAsync<T>(string cacheKey, T data, TimeSpan? expiration = null);

    Task RemoveAsync(string cacheKey);

    Task RemoveMultipleAsync(IEnumerable<string> cacheKeys);
}