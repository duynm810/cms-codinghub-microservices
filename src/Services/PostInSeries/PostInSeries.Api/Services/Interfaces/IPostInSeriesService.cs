using Shared.Dtos.PostInSeries;
using Shared.Responses;

namespace PostInSeries.Api.Services.Interfaces;

public interface IPostInSeriesService
{
    Task<ApiResult<bool>> CreatePostToSeries(CreatePostInSeriesDto request);

    Task<ApiResult<bool>> DeletePostToSeries(DeletePostInSeriesDto request);
}