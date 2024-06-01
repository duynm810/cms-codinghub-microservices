using MediatR;
using Post.Application.Commons.Models;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByNonStaticPageCategory;

public class GetPostsByNonStaticPageCategoryQuery : IRequest<ApiResult<IEnumerable<CategoryWithPostsModel>>>
{
    
}