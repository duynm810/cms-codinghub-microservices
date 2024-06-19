using MediatR;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByNonStaticPageCategory;

public class GetPostsByNonStaticPageCategoryQuery(int count) : IRequest<ApiResult<IEnumerable<PostsByNonStaticPageCategoryDto>>>
{
    public int Count { get; set; } = count;
}