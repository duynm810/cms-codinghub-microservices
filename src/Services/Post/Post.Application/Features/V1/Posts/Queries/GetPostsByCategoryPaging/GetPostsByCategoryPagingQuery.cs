using MediatR;
using Shared.Dtos.Post;
using Shared.Requests.Post;
using Shared.Requests.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByCategoryPaging;

public class GetPostsByCategoryPagingQuery(string categorySlug, GetPostsByCategoryRequest request)
    : IRequest<ApiResult<PostsByCategoryDto>>
{
    public string CategorySlug { get; set; } = categorySlug;

    public GetPostsByCategoryRequest Request { get; set; } = request;
}