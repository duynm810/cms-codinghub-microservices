using MediatR;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPosts;

public class GetPostsQuery : IRequest<ApiResult<IEnumerable<PostDto>>>
{
}