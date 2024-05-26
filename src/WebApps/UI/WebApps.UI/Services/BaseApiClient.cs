using System.Net.Http.Headers;
using Contracts.Commons.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Shared.Responses;
using Shared.Settings;
using WebApps.UI.Services.Interfaces;

namespace WebApps.UI.Services;

public class BaseApiClient(ISerializeService serializeService, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ApiSettings apiSettings) : IBaseApiClient
{
    private async Task<HttpClient> CreateClientAsync(bool requiredLogin)
    {
        if (string.IsNullOrEmpty(apiSettings.ServerUrl))
        {
            throw new InvalidOperationException("Server URL must be configured.");
        }

        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(apiSettings.ServerUrl);

        if (requiredLogin && httpContextAccessor.HttpContext != null)
        {
            var token = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }
    
    public async Task<ApiResult<List<T>>> GetListAsync<T>(string url, bool requiredLogin = false)
    {
        var client = await CreateClientAsync(requiredLogin);
        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Request failed with status code {response.StatusCode} and content {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = serializeService.Deserialize<ApiResult<List<T>>>(responseContent);

        if (result == null)
        {
            throw new InvalidOperationException("Failed to deserialize the response content.");
        }

        return result;
    }
}