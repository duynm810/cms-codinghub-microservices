using Shared.Dtos.PostActivity;
using Shared.Responses;
using WebApps.UI.ApiClients.Interfaces;

namespace WebApps.UI.ApiClients;

public class PostActivityLogApiClient(IBaseApiClient baseApiClient) : IPostActivityLogApiClient
{
    public async Task<ApiResult<List<PostActivityLogDto>>> GetActivityLogs(Guid postId)
    {
        return await baseApiClient.GetListAsync<PostActivityLogDto>($"/posts/activity-logs/{postId}", true);
    }
}