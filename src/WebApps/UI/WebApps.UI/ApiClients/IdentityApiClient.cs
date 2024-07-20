using Shared.Dtos.Identity.User;
using Shared.Responses;
using WebApps.UI.ApiClients.Interfaces;

namespace WebApps.UI.ApiClients;

public class IdentityApiClient(IBaseApiClient baseApiClient) : IIdentityApiClient
{
    public async Task<ApiResult<UserDto>> GetMe()
    {
        return await baseApiClient.GetAsync<UserDto>($"/users/me");
    }
}