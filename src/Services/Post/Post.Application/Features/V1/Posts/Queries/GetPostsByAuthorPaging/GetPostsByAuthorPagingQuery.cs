using MediatR;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByAuthorPaging;

public class GetPostsByAuthorPagingQuery(string userName, int pageNumber, int pageSize) : IRequest<ApiResult<PostsByAuthorDto>>
{
    public string UserName { get; set; } = userName;

    public int PageNumber { get; set; } = pageNumber;

    public int PageSize { get; set; } = pageSize;
}