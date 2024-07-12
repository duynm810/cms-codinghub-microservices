using MediatR;
using Shared.Dtos.Identity.User;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByCurrentUserPaging;

public class GetPostsByCurrentUserPagingQuery(CurrentUserDto currentUser, int pageNumber, int pageSize) : IRequest<ApiResult<PagedResponse<PostDto>>>
{
    public CurrentUserDto CurrentUser { get; set; } = currentUser;

    public int PageNumber { get; set; } = pageNumber;

    public int PageSize { get; set; } = pageSize;
}