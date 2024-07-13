using Shared.Requests.PostInSeries;
using Shared.Responses;

namespace PostInSeries.Api.Services.Interfaces;

public interface IPostInSeriesService
{
    Task<ApiResult<bool>> CreatePostToSeries(CreatePostInSeriesRequest request);

    Task<ApiResult<bool>> DeletePostToSeries(DeletePostInSeriesRequest request);
}