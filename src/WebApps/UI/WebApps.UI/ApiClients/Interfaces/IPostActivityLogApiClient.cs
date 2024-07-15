using Shared.Dtos.PostActivity;
using Shared.Responses;

namespace WebApps.UI.ApiClients.Interfaces;

public interface IPostActivityLogApiClient
{
    Task<ApiResult<List<PostActivityLogDto>>> GetActivityLogs(Guid postId);
}