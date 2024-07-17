using Shared.Dtos.PostInSeries;
using Shared.Requests.PostInSeries;
using Shared.Responses;

namespace PostInSeries.Api.Services.Interfaces;

public interface IPostInSeriesService
{
    Task<ApiResult<bool>> CreatePostsToSeries(CreatePostInSeriesRequest request);

    Task<ApiResult<bool>> DeletePostToSeries(DeletePostInSeriesRequest request);

    Task<ApiResult<ManagePostInSeriesDto>> GetSeriesForPost(Guid postId);
}