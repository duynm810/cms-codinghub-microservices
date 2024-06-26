using MediatR;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetMostCommentPosts;

public class GetMostCommentPostsQuery(int count) : IRequest<ApiResult<IEnumerable<PostDto>>>
{
    public int Count { get; set; } = count;
}