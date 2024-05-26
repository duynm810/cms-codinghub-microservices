using Shared.Responses;

namespace WebApps.UI.Services.Interfaces;

public interface IBaseApiClient
{
    Task<ApiResult<List<T>>> GetListAsync<T>(string url, bool requiredLogin = false);
}