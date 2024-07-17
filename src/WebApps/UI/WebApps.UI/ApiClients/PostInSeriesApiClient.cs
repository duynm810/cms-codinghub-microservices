using Shared.Dtos.PostInSeries;
using Shared.Requests.PostInSeries;
using Shared.Responses;
using WebApps.UI.ApiClients.Interfaces;

namespace WebApps.UI.ApiClients;

public class PostInSeriesApiClient(IBaseApiClient baseApiClient) : IPostInSeriesApiClient
{
    public async Task<ApiResult<bool>> CreatePostsToSeries(CreatePostInSeriesRequest request)
    {
        return await baseApiClient.PostAsync<CreatePostInSeriesRequest, bool>($"/post-in-series", request, true);
    }
    
    public async Task<ApiResult<ManagePostInSeriesDto>> GetSeriesForPost(Guid postId)
    {
        return await baseApiClient.GetAsync<ManagePostInSeriesDto>($"/post-in-series/{postId}/manage-series", true);
    }
}