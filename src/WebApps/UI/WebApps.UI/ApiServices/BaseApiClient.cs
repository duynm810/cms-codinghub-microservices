using System.Net.Http.Headers;
using System.Text;
using Contracts.Commons.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Shared.Constants;
using Shared.Responses;
using Shared.Settings;
using WebApps.UI.ApiServices.Interfaces;

namespace WebApps.UI.ApiServices;

public class BaseApiClient(
    ISerializeService serializeService,
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    ApiSettings apiSettings) : IBaseApiClient
{
    #region CRUD

    public async Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest data,
        bool requiredLogin = false)
    {
        var client = await CreateClientAsync(requiredLogin);
        var jsonContent = serializeService.Serialize(data);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(string.Format(ErrorMessagesConsts.Network.RequestFailed, response.StatusCode,
                errorContent));
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = serializeService.Deserialize<ApiResult<TResponse>>(responseContent);

        if (result == null)
        {
            throw new InvalidOperationException(ErrorMessagesConsts.Data.DeserializeFailed);
        }

        return result;
    }

    public async Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest data,
        bool requiredLogin = false)
    {
        var client = await CreateClientAsync(requiredLogin);
        var jsonContent = serializeService.Serialize(data);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await client.PutAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(string.Format(ErrorMessagesConsts.Network.RequestFailed, response.StatusCode,
                errorContent));
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = serializeService.Deserialize<ApiResult<TResponse>>(responseContent);

        if (result == null)
        {
            throw new InvalidOperationException(ErrorMessagesConsts.Data.DeserializeFailed);
        }

        return result;
    }

    public async Task<ApiResult<TResponse>> DeleteAsync<TResponse>(string url, bool requiredLogin = false)
    {
        var client = await CreateClientAsync(requiredLogin);
        var response = await client.DeleteAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(string.Format(ErrorMessagesConsts.Network.RequestFailed, response.StatusCode,
                errorContent));
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = serializeService.Deserialize<ApiResult<TResponse>>(responseContent);

        if (result == null)
        {
            throw new InvalidOperationException(ErrorMessagesConsts.Data.DeserializeFailed);
        }

        return result;
    }

    public async Task<ApiResult<List<T>>> GetListAsync<T>(string url, bool requiredLogin = false)
    {
        var client = await CreateClientAsync(requiredLogin);
        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(string.Format(ErrorMessagesConsts.Network.RequestFailed, response.StatusCode,
                errorContent));
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = serializeService.Deserialize<ApiResult<List<T>>>(responseContent);

        if (result == null)
        {
            throw new InvalidOperationException(ErrorMessagesConsts.Data.DeserializeFailed);
        }

        return result;
    }

    public async Task<ApiResult<T>> GetAsync<T>(string url, bool requiredLogin = false)
    {
        var client = await CreateClientAsync(requiredLogin);
        var response = await client.GetAsync(url);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = serializeService.Deserialize<ApiResult<T>>(responseContent);

        if (result == null)
        {
            throw new InvalidOperationException(ErrorMessagesConsts.Data.DeserializeFailed);
        }

        return result;
    }

    #endregion

    #region HELPERS

    public async Task<HttpClient> CreateClientAsync(bool requiredLogin)
    {
        if (string.IsNullOrEmpty(apiSettings.ServerUrl))
        {
            throw new InvalidOperationException(ErrorMessagesConsts.Network.ServerUrlNotConfigured);
        }

        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(apiSettings.ServerUrl);

        if (!requiredLogin || httpContextAccessor.HttpContext == null)
        {
            return client;
        }

        var token = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    #endregion
}