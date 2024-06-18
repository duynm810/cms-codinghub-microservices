using MediatR;
using Post.Application.Commons.Models;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByCurrentUserPaging;

public class GetPostsByCurrentUserPagingQuery(Guid currentUserId, int pageNumber, int pageSize) : IRequest<ApiResult<PagedResponse<PostModel>>>
{
    public Guid CurrentUserId { get; set; } = currentUserId;

    public int PageNumber { get; set; } = pageNumber;

    public int PageSize { get; set; } = pageSize;
}