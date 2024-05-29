using Shared.Dtos.Post;
using Shared.Responses;
using WebApps.UI.Services.Interfaces;

namespace WebApps.UI.Services;

public class PostApiClient(IBaseApiClient baseApiClient) : IPostApiClient
{
    public async Task<ApiResult<List<FeaturedPostDto>>> GetFeaturedPosts(int count)
    {
        return await baseApiClient.GetListAsync<FeaturedPostDto>($"/posts/featured?count={count}");
    }

    public async Task<ApiResult<PagedResponse<PostByCategoryDto>>> GetPostsByCategory(string categorySlug,
        int pageNumber, int pageSize)
    {
        return await baseApiClient.GetAsync<PagedResponse<PostByCategoryDto>>(
            $"/posts/by-category/{categorySlug}/paging?pageNumber={pageNumber}&pageSize={pageSize}");
    }

    public async Task<ApiResult<PostDetailDto>> GetPostBySlug(string slug)
    {
        return await baseApiClient.GetAsync<PostDetailDto>($"/posts/by-slug/{slug}");
    }
}