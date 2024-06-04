using Shared.Dtos.PostInSeries;
using Shared.Responses;

namespace PostInSeries.Api.Services.Interfaces;

public interface IPostInSeriesService
{
    Task<ApiResult<bool>> CreatePostToSeries(CreatePostInSeriesDto request);

    Task<ApiResult<bool>> DeletePostToSeries(DeletePostInSeriesDto request);

    Task<ApiResult<IEnumerable<PostInSeriesDto>>> GetPostsInSeries(Guid seriesId);

    Task<ApiResult<IEnumerable<PostInSeriesDto>>> GetPostsInSeriesBySlug(string seriesSlug);

    Task<ApiResult<PagedResponse<PostInSeriesDto>>> GetPostsInSeriesPaging(Guid seriesId, int pageNumber, int pageSize);

    Task<ApiResult<PagedResponse<PostInSeriesDto>>> GetPostsInSeriesBySlugPaging(string seriesSlug, int pageNumber,
        int pageSize);
}