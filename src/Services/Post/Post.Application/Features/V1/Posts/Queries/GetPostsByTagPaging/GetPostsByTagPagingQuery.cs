using MediatR;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByTagPaging;

public class GetPostsByTagPagingQuery(string tagSlug, int pageNumber, int pageSize) : IRequest<ApiResult<PostsByTagDto>>
{
    public string TagSlug { get; set; } = tagSlug;

    public int PageNumber { get; set; } = pageNumber;

    public int PageSize { get; set; } = pageSize;
}