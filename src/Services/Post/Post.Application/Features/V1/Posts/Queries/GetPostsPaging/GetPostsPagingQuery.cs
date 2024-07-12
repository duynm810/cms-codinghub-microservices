using MediatR;
using Shared.Dtos.Post;
using Shared.Requests.Post;
using Shared.Requests.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsPaging;

public class GetPostsPagingQuery(GetPostsRequest request) : IRequest<ApiResult<PagedResponse<PostDto>>>
{
    public GetPostsRequest Request { get; set; } = request;
}