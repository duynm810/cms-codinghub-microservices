using MediatR;
using Post.Application.Commons.Models;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsPaging;

public class GetPostsPagingQuery(int pageNumber = 1, int pageSize = 10) : IRequest<ApiResult<PagedResponse<PostDto>>>
{
    public int PageNumber { get; set; } = pageNumber;

    public int PageSize { get; set; } = pageSize;
}