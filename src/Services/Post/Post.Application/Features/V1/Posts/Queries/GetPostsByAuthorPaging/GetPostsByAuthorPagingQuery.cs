using MediatR;
using Post.Application.Commons.Models;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByAuthorPaging;

public class GetPostsByAuthorPagingQuery(Guid authorId, int pageNumber, int pageSize) : IRequest<ApiResult<PagedResponse<PostModel>>>
{
    public Guid AuthorId { get; set; } = authorId;

    public int PageNumber { get; set; } = pageNumber;

    public int PageSize { get; set; } = pageSize;
}