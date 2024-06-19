using MediatR;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByAuthorPaging;

public class GetPostsByAuthorPagingQuery(Guid authorId, int pageNumber, int pageSize) : IRequest<ApiResult<PostsByAuthorDto>>
{
    public Guid AuthorId { get; set; } = authorId;

    public int PageNumber { get; set; } = pageNumber;

    public int PageSize { get; set; } = pageSize;
}