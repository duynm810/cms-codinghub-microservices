using MediatR;
using Post.Application.Commons.Models;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPosts;

public class GetPostsQuery : IRequest<ApiResult<IEnumerable<PostDto>>>
{
    
}