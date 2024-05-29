using MediatR;
using Post.Application.Commons.Models;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetRelatedPosts;

public class GetRelatedPostsQuery(Guid id, int count) : IRequest<ApiResult<IEnumerable<PostDto>>>
{
    public Guid Id { get; } = id;
    
    public int Count { get; } = count;
}