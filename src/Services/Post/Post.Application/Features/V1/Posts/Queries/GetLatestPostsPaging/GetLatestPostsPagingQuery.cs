using MediatR;
using Post.Application.Commons.Models;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetLatestPostsPaging;

public class GetLatestPostsPagingQuery(int pageNumber, int pageSize) : IRequest<ApiResult<PagedResponse<PostModel>>>
{
    public int PageNumber { get; set; } = pageNumber;

    public int PageSize { get; set; } = pageSize;
}