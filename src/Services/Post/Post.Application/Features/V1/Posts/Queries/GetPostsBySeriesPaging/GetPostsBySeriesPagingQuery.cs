using MediatR;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsBySeriesPaging;

public class GetPostsBySeriesPagingQuery(string seriesSlug, int pageNumber, int pageSize) : IRequest<ApiResult<PostsBySeriesDto>>
{
    public string SeriesSlug { get; set; } = seriesSlug;

    public int PageNumber { get; set; } = pageNumber;

    public int PageSize { get; set; } = pageSize;
}