using MediatR;
using Shared.Dtos.Identity.User;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByCurrentUserPaging;

public class GetPostsByCurrentUserPagingQuery(GetPostsByCurrentUserDto filter, CurrentUserDto currentUser) : IRequest<ApiResult<PagedResponse<PostDto>>>
{
    public GetPostsByCurrentUserDto Filter { get; set; } = filter;
    
    public CurrentUserDto CurrentUser { get; set; } = currentUser;
}