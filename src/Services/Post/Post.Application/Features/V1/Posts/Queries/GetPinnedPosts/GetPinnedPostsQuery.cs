using MediatR;
using Shared.Dtos.Post;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPinnedPosts;

public class GetPinnedPostsQuery(int count) : IRequest<ApiResult<IEnumerable<PostDto>>>
{
    public int Count { get; set; } = count;
}