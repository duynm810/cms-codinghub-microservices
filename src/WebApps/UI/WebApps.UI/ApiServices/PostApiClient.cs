using Shared.Dtos.Post;
using Shared.Dtos.Post.Queries;
using Shared.Dtos.PostInSeries;
using Shared.Dtos.PostInTag;
using Shared.Responses;
using WebApps.UI.ApiServices.Interfaces;

namespace WebApps.UI.ApiServices;

public class PostApiClient(IBaseApiClient baseApiClient) : IPostApiClient
{
    public async Task<ApiResult<List<PostDto>>> GetFeaturedPosts(int count)
    {
        return await baseApiClient.GetListAsync<PostDto>($"/posts/featured?count={count}");
    }

    public async Task<ApiResult<List<PostDto>>> GetPinnedPosts(int count)
    {
        return await baseApiClient.GetListAsync<PostDto>($"/posts/pinned?count={count}");
    }

    public async Task<ApiResult<PagedResponse<PostDto>>> GetPostsByCategoryPaging(string categorySlug,
        int pageNumber, int pageSize)
    {
        return await baseApiClient.GetAsync<PagedResponse<PostDto>>(
            $"/posts/by-category/{categorySlug}/paging?pageNumber={pageNumber}&pageSize={pageSize}");
    }
    
    public async Task<ApiResult<PostsByAuthorDto>> GetPostsByAuthorPaging(string userName,
        int pageNumber, int pageSize)
    {
        return await baseApiClient.GetAsync<PostsByAuthorDto>(
            $"/posts/by-author/{userName}/paging?pageNumber={pageNumber}&pageSize={pageSize}");
    }
    
    public async Task<ApiResult<PagedResponse<PostDto>>> GetPostsByCurrentUserPaging(int pageNumber, int pageSize)
    {
        return await baseApiClient.GetAsync<PagedResponse<PostDto>>(
            $"/posts/by-current-user/paging?pageNumber={pageNumber}&pageSize={pageSize}", true);
    }
    
    public async Task<ApiResult<PostsBySlugDto>> GetPostBySlug(string slug, int relatedCount)
    {
        return await baseApiClient.GetAsync<PostsBySlugDto>($"/posts/by-slug/{slug}?relatedCount={relatedCount}");
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

    public async Task<ApiResult<PagedResponse<PostInTagDto>>> GetPostsInTagBySlugPaging(string tagSlug, int pageNumber, int pageSize)
    {
        return await baseApiClient.GetAsync<PagedResponse<PostInTagDto>>(
            $"/post-in-tag/by-slug/{tagSlug}/paging?pageNumber={pageNumber}&pageSize={pageSize}");
    }
    
    public async Task<ApiResult<List<PostDto>>> GetMostCommentedPosts(int count)
    {
        return await baseApiClient.GetListAsync<PostDto>($"/posts/most-commented?count={count}");
    }

    public async Task<ApiResult<List<PostDto>>> GetMostLikedPosts(int count)
    {
        return await baseApiClient.GetListAsync<PostDto>($"/posts/most-liked?count={count}");
    }

    public async Task<ApiResult<List<PostsByNonStaticPageCategoryDto>>> GetPostsByNonStaticPageCategory(int count)
    {
        return await baseApiClient.GetListAsync<PostsByNonStaticPageCategoryDto>($"/posts/by-non-static-page-category?count={count}");
    }
    
}