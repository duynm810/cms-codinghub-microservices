using Shared.Dtos.Identity.User;
using Shared.Requests.Identity.User;
using Shared.Responses;
using WebApps.UI.ApiClients.Interfaces;

namespace WebApps.UI.ApiClients;

public class IdentityApiClient(IBaseApiClient baseApiClient) : IIdentityApiClient
{
    #region Users

    public async Task<ApiResult<UserDto>> GetMe()
    {
        return await baseApiClient.GetAsync<UserDto>($"/users/me", true);
    }

    public async Task<ApiResult<bool>> UpdateUser(Guid userId, UpdateUserRequest request)
    {
        return await baseApiClient.PutAsync<UpdateUserRequest, bool>($"/users/{userId}", request, true);
    }

    #endregion
}