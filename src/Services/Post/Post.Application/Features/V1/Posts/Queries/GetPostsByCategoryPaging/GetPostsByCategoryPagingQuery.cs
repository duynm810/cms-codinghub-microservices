using MediatR;
using Post.Application.Commons.Models;
using Shared.Dtos.Post;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByCategoryPaging;

public class GetPostsByCategoryPagingQuery(string categorySlug, int pageNumber, int pageSize)
    : IRequest<ApiResult<PagedResponse<PostModel>>>
{
    public string CategorySlug { get; set; } = categorySlug;

    public int PageNumber { get; set; } = pageNumber;

    public int PageSize { get; set; } = pageSize;
}