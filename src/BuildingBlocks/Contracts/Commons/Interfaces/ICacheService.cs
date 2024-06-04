namespace Contracts.Commons.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string cacheKey, CancellationToken cancellationToken = default);

    Task SetAsync<T>(string cacheKey, T data, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default);

    Task RemoveMultipleAsync(IEnumerable<string> cacheKeys, CancellationToken cancellationToken = default);
}