using Shared.Dtos.Identity.User;
using Shared.Requests.Identity.User;
using Shared.Responses;
using WebApps.UI.ApiClients.Interfaces;

namespace WebApps.UI.ApiClients;

public class IdentityApiClient(IBaseApiClient baseApiClient) : IIdentityApiClient
{
    #region Users
    
    public async Task<ApiResult<bool>> UpdateUser(Guid userId, UpdateUserRequest request)
    {
        return await baseApiClient.PutAsync<UpdateUserRequest, bool>($"/users/{userId}", request, true);
    }

    public async Task<ApiResult<UserDto>?> GetUserById(Guid userId)
    {
        return await baseApiClient.GetAsync<UserDto>($"/users/{userId}", true);
    }
    
    public async Task<ApiResult<UserDto>> GetMe()
    {
        return await baseApiClient.GetAsync<UserDto>($"/users/me", true);
    }
    
    public async Task<ApiResult<bool>> UpdateAvatar(Guid userId, UpdateAvatarRequest request)
    {
        return await baseApiClient.PutAsync<UpdateAvatarRequest, bool>($"/users/{userId}/update-avatar", request, true);
    }

    #endregion
}