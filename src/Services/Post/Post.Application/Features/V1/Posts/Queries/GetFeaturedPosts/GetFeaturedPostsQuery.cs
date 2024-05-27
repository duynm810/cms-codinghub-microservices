using MediatR;
using Post.Application.Commons.Models;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetFeaturedPosts;

public class GetFeaturedPostsQuery(int count) : IRequest<ApiResult<IEnumerable<PostDto>>>
{
    public int Count { get; } = count;
}