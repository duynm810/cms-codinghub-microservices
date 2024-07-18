using Shared.Dtos.PostInSeries;
using Shared.Requests.PostInSeries;
using Shared.Responses;

namespace WebApps.UI.ApiClients.Interfaces;

public interface IPostInSeriesApiClient
{
    Task<ApiResult<bool>> CreatePostToSeries(CreatePostInSeriesRequest request);

    Task<ApiResult<ManagePostInSeriesDto>> GetSeriesForPost(Guid postId);
}