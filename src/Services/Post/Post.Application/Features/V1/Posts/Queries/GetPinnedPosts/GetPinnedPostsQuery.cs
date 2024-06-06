using MediatR;
using Post.Application.Commons.Models;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPinnedPosts;

public class GetPinnedPostsQuery(int count) : IRequest<ApiResult<IEnumerable<PostModel>>>
{
    public int Count { get; set; } = count;
}