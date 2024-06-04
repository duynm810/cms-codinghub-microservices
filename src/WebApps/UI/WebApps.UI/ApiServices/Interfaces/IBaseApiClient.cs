using Shared.Responses;

namespace WebApps.UI.ApiServices.Interfaces;

public interface IBaseApiClient
{
    Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest data, bool requiredLogin = false);

    Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest data, bool requiredLogin = false);

    Task<ApiResult<TResponse>> DeleteAsync<TResponse>(string url, bool requiredLogin = false);

    Task<ApiResult<List<T>>> GetListAsync<T>(string url, bool requiredLogin = false);

    Task<ApiResult<T>> GetAsync<T>(string url, bool requiredLogin = false);
}