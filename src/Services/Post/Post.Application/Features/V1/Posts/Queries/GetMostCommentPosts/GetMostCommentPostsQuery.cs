using MediatR;
using Post.Application.Commons.Models;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetMostCommentPosts;

public class GetMostCommentPostsQuery(int count) : IRequest<ApiResult<IEnumerable<PostModel>>>
{
    public int Count { get; set; } = count;
}