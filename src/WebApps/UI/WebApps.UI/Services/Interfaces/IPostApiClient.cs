using Shared.Dtos.Post;
using Shared.Responses;

namespace WebApps.UI.Services.Interfaces;

public interface IPostApiClient
{
    Task<ApiResult<List<FeaturedPostDto>>> GetFeaturedPosts();

    Task<ApiResult<PagedResponse<PostByCategoryDto>>> GetPostsByCategory(string categorySlug, int pageNumber, int pageSize);
    
    Task<ApiResult<PostDetailDto>> GetPostBySlug(string slug);
}