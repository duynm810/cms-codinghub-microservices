using MediatR;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsPaging;

public class GetPostsPagingQuery(string? filter, int pageNumber, int pageSize) : IRequest<ApiResult<PagedResponse<PostDto>>>
{
    public string? Filter { get; set; } = filter;

    public int PageNumber { get; set; } = pageNumber;

    public int PageSize { get; set; } = pageSize;
}