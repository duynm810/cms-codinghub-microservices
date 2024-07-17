using Shared.Requests.PostInSeries;
using Shared.Responses;

namespace WebApps.UI.ApiClients.Interfaces;

public interface IPostInSeriesApiClient
{
    Task<ApiResult<bool>> CreatePostsToSeries(CreatePostInSeriesRequest request);
}