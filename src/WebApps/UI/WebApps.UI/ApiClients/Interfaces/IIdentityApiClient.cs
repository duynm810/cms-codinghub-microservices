using Shared.Dtos.Identity.User;
using Shared.Requests.Identity.User;
using Shared.Responses;

namespace WebApps.UI.ApiClients.Interfaces;

public interface IIdentityApiClient
{
    #region Users
    
    Task<ApiResult<bool>> UpdateUser(Guid userId, UpdateUserRequest request);

    Task<ApiResult<UserDto>?> GetUserById(Guid userId);
    
    Task<ApiResult<UserDto>> GetMe();

    #endregion
}