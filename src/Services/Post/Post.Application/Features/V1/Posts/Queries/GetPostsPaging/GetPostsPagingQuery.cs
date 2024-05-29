using MediatR;
using Post.Application.Commons.Models;
using Shared.Dtos.Post;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsPaging;

public class GetPostsPagingQuery(string? filter, int pageNumber, int pageSize) : IRequest<ApiResult<PagedResponse<PostModel>>>
{
    public string? Filter { get; set; } = filter;
    
    public int PageNumber { get; set; } = pageNumber;

    public int PageSize { get; set; } = pageSize;
}