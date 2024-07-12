using MediatR;
using Shared.Dtos.Post;
using Shared.Requests.Post;
using Shared.Requests.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsBySeriesPaging;

public class GetPostsBySeriesPagingQuery(string seriesSlug, GetPostBySeriesRequest request) : IRequest<ApiResult<PostsBySeriesDto>>
{
    public string SeriesSlug { get; set; } = seriesSlug;

    public GetPostBySeriesRequest Request { get; set; } = request;

}