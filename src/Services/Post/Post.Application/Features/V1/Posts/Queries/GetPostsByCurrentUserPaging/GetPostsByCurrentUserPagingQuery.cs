using MediatR;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByCurrentUserPaging;

public class GetPostsByCurrentUserPagingQuery(Guid currentUserId, int pageNumber, int pageSize) : IRequest<ApiResult<PagedResponse<PostDto>>>
{
    public Guid CurrentUserId { get; set; } = currentUserId;

    public int PageNumber { get; set; } = pageNumber;

    public int PageSize { get; set; } = pageSize;
}