using MediatR;
using Shared.Dtos.Post;
using Shared.Requests.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsBySeriesPaging;

public class GetPostsBySeriesPagingQuery(string seriesSlug, GetPostsBySeriesRequest request) : IRequest<ApiResult<PostsBySeriesDto>>
{
    public string SeriesSlug { get; set; } = seriesSlug;

    public GetPostsBySeriesRequest Request { get; set; } = request;

}