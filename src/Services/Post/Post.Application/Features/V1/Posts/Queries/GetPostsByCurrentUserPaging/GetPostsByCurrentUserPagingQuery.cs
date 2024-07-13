using MediatR;
using Shared.Dtos.Identity.User;
using Shared.Dtos.Post;
using Shared.Requests.Post;
using Shared.Requests.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByCurrentUserPaging;

public class GetPostsByCurrentUserPagingQuery(GetPostsByCurrentUserRequest request, CurrentUserDto currentUser) : IRequest<ApiResult<PagedResponse<PostDto>>>
{
    public GetPostsByCurrentUserRequest Request { get; set; } = request;
    
    public CurrentUserDto CurrentUser { get; set; } = currentUser;
}