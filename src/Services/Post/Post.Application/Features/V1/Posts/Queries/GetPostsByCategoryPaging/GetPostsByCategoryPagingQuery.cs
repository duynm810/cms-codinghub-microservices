using MediatR;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByCategoryPaging;

public class GetPostsByCategoryPagingQuery(string categorySlug, int pageNumber, int pageSize)
    : IRequest<ApiResult<PagedResponse<PostDto>>>
{
    public string CategorySlug { get; set; } = categorySlug;

    public int PageNumber { get; set; } = pageNumber;

    public int PageSize { get; set; } = pageSize;
}