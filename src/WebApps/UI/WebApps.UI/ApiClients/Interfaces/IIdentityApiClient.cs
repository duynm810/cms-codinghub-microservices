using Shared.Dtos.Identity.User;
using Shared.Responses;

namespace WebApps.UI.ApiClients.Interfaces;

public interface IIdentityApiClient
{
    Task<ApiResult<UserDto>> GetMe();
}