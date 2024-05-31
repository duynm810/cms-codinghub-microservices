using Shared.Dtos.Post;
using Shared.Dtos.PostInSeries;
using Shared.Responses;
using WebApps.UI.Services.Interfaces;

namespace WebApps.UI.Services;

public class PostApiClient(IBaseApiClient baseApiClient) : IPostApiClient
{
    public async Task<ApiResult<List<PostDto>>> GetFeaturedPosts()
    {
        return await baseApiClient.GetListAsync<PostDto>($"/posts/featured");
    }

    public async Task<ApiResult<PagedResponse<PostDto>>> GetPostsByCategoryPaging(string categorySlug,
        int pageNumber, int pageSize)
    {
        return await baseApiClient.GetAsync<PagedResponse<PostDto>>(
            $"/posts/by-category/{categorySlug}/paging?pageNumber={pageNumber}&pageSize={pageSize}");
    }

    public async Task<ApiResult<PostDetailDto>> GetPostBySlug(string slug)
    {
        return await baseApiClient.GetAsync<PostDetailDto>($"/posts/by-slug/{slug}");
    }
    
    public async Task<ApiResult<PagedResponse<PostDto>>> GetLatestPostsPaging(int pageNumber, int pageSize)
    {
        return await baseApiClient.GetAsync<PagedResponse<PostDto>>(
            $"/posts/latest/paging?pageNumber={pageNumber}&pageSize={pageSize}");
    }

    public async Task<ApiResult<PagedResponse<PostDto>>> SearchPostsPaging(string keyword, int pageNumber, int pageSize)
    {
        if (!string.IsNullOrEmpty(keyword))
            return await baseApiClient.GetAsync<PagedResponse<PostDto>>(
                $"/posts/paging?filter={keyword}&pageNumber={pageNumber}&pageSize={pageSize}");

        return await baseApiClient.GetAsync<PagedResponse<PostDto>>(
            $"/posts/paging?pageNumber={pageNumber}&pageSize={pageSize}");
    }

    public async Task<ApiResult<PagedResponse<PostInSeriesDto>>> GetPostsInSeriesBySlugPaging(string seriesSlug,
        int pageNumber, int pageSize)
    {
        return await baseApiClient.GetAsync<PagedResponse<PostInSeriesDto>>(
            $"/post-in-series/by-slug/{seriesSlug}/paging?pageNumber={pageNumber}&pageSize={pageSize}");
    }
}