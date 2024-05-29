using Shared.Dtos.Post;
using Shared.Responses;

namespace WebApps.UI.Services.Interfaces;

public interface IPostApiClient
{
    Task<ApiResult<List<PostDto>>> GetFeaturedPosts();

    Task<ApiResult<PagedResponse<PostDto>>> GetPostsByCategory(string categorySlug, int pageNumber, int pageSize);
    
    Task<ApiResult<PostDetailDto>> GetPostBySlug(string slug);

    Task<ApiResult<PagedResponse<PostDto>>> GetLatestPosts(int pageNumber, int pageSize);

    Task<ApiResult<PagedResponse<PostDto>>> SearchPosts(string keyword, int pageNumber, int pageSize);
}