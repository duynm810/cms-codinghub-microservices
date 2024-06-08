using MediatR;
using Post.Application.Commons.Models;
using Shared.Dtos.Post;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetFeaturedPosts;

public class GetFeaturedPostsQuery(int count) : IRequest<ApiResult<IEnumerable<PostModel>>>
{
    public int Count { get; set; } = count;
}