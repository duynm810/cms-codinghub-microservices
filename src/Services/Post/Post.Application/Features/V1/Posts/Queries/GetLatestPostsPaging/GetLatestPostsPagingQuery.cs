using MediatR;
using Shared.Dtos.Post;
using Shared.Requests.Post;
using Shared.Requests.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetLatestPostsPaging;

public class GetLatestPostsPagingQuery(GetLatestPostsRequest request) : IRequest<ApiResult<PagedResponse<PostDto>>>
{
    public GetLatestPostsRequest Request { get; set; } = request;
}