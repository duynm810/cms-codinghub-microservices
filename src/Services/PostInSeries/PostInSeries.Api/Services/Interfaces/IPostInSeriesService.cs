using Shared.Dtos.PostInSeries;
using Shared.Responses;

namespace PostInSeries.Api.Services.Interfaces;

public interface IPostInSeriesService
{
    Task<ApiResult<bool>> CreatePostToSeries(Guid seriesId, Guid postId, int sortOrder);

    Task<ApiResult<bool>> DeletePostToSeries(Guid seriesId, Guid postId);

    Task<ApiResult<IEnumerable<PostInSeriesDto>>> GetPostsInSeries(Guid seriesId);

    Task<ApiResult<IEnumerable<PostInSeriesDto>>> GetPostsInSeriesBySlug(string seriesSlug);
    
    Task<ApiResult<PagedResponse<PostInSeriesDto>>> GetPostsInSeriesPaging(Guid seriesId, int pageNumber, int pageSize);
}