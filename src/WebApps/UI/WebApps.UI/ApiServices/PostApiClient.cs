using Shared.Dtos.Post;
using Shared.Dtos.Post.Commands;
using Shared.Dtos.Post.Queries;
using Shared.Dtos.PostInSeries;
using Shared.Dtos.PostInTag;
using Shared.Responses;
using WebApps.UI.ApiServices.Interfaces;

namespace WebApps.UI.ApiServices;

public class PostApiClient(IBaseApiClient baseApiClient) : IPostApiClient
{
    public async Task<ApiResult<Guid>> CreatePost(CreatePostDto request)
    {
        return await baseApiClient.PostAsync<CreatePostDto, Guid>($"/posts", request, true);
    }

    public async Task<ApiResult<bool>> UpdatePost(Guid id, UpdatePostDto request)
    {
        return await baseApiClient.PutAsync<UpdatePostDto, bool>($"/posts/{id}", request, true);
    }

    public async Task<ApiResult<bool>> UpdateThumbnail(Guid id, UpdateThumbnailDto request)
    {
        return await baseApiClient.PutAsync<UpdateThumbnailDto, bool>($"/posts/update-thumbnail/{id}", request, true);
    }

    public async Task<ApiResult<bool>> DeletePost(Guid id)
    {
        return await baseApiClient.DeleteAsync<bool>($"/posts/{id}", true);
    }
    
    public async Task<ApiResult<PostDto>> GetPostBySlug(string slug)
    {
        return await baseApiClient.GetAsync<PostDto>($"/posts/slug/{slug}", true);
    }

    public async Task<ApiResult<PostsByCategoryDto>> GetPostsByCategoryPaging(string categorySlug, int pageNumber, int pageSize)
    {
        return await baseApiClient.GetAsync<PostsByCategoryDto>(
            $"/posts/by-category/{categorySlug}/paging?pageNumber={pageNumber}&pageSize={pageSize}");
    }
    
    public async Task<ApiResult<PostsBySeriesDto>> GetPostsBySeriesPaging(string seriesSlug, int pageNumber, int pageSize)
    {
        return await baseApiClient.GetAsync<PostsBySeriesDto>(
            $"/posts/by-series/{seriesSlug}/paging?pageNumber={pageNumber}&pageSize={pageSize}");
    }

    public async Task<ApiResult<PostsByTagDto>> GetPostsByTagPaging(string tagSlug, int pageNumber, int pageSize)
    {
        return await baseApiClient.GetAsync<PostsByTagDto>(
            $"/posts/by-tag/{tagSlug}/paging?pageNumber={pageNumber}&pageSize={pageSize}");
    }
    
    public async Task<ApiResult<PostsByAuthorDto>> GetPostsByAuthorPaging(string userName, int pageNumber, int pageSize)
    {
        return await baseApiClient.GetAsync<PostsByAuthorDto>(
            $"/posts/by-author/{userName}/paging?pageNumber={pageNumber}&pageSize={pageSize}");
    }
    
    public async Task<ApiResult<PagedResponse<PostDto>>> GetPostsByCurrentUserPaging(int pageNumber, int pageSize)
    {
        return await baseApiClient.GetAsync<PagedResponse<PostDto>>(
            $"/posts/by-current-user/paging?pageNumber={pageNumber}&pageSize={pageSize}", true);
    }
    
    public async Task<ApiResult<PostsBySlugDto>> GetDetailBySlug(string slug, int relatedCount)
    {
        return await baseApiClient.GetAsync<PostsBySlugDto>($"/posts/detail/by-slug/{slug}?relatedCount={relatedCount}");
    }

    public async Task<ApiResult<PagedResponse<PostDto>>> SearchPostsPaging(string keyword, int pageNumber, int pageSize)
    {
        if (!string.IsNullOrEmpty(keyword))
            return await baseApiClient.GetAsync<PagedResponse<PostDto>>(
                $"/posts/paging?filter={keyword}&pageNumber={pageNumber}&pageSize={pageSize}");

        return await baseApiClient.GetAsync<PagedResponse<PostDto>>(
            $"/posts/paging?pageNumber={pageNumber}&pageSize={pageSize}");
    }
    
    public async Task<ApiResult<List<PostDto>>> GetMostCommentedPosts(int count)
    {
        return await baseApiClient.GetListAsync<PostDto>($"/posts/most-commented?count={count}");
    }

    public async Task<ApiResult<List<PostsByNonStaticPageCategoryDto>>> GetPostsByNonStaticPageCategory(int count)
    {
        return await baseApiClient.GetListAsync<PostsByNonStaticPageCategoryDto>($"/posts/by-non-static-page-category?count={count}");
    }
    
    public async Task<ApiResult<bool>> TogglePinStatus(Guid id, TogglePinStatusDto request)
    {
        return await baseApiClient.PutAsync<TogglePinStatusDto, bool>($"/posts/toggle-pin-status/{id}", request, true);
    }
    
    public async Task<ApiResult<bool>> ToggleFeaturedStatus(Guid id, ToggleFeaturedStatusDto request)
    {
        return await baseApiClient.PutAsync<ToggleFeaturedStatusDto, bool>($"/posts/toggle-featured-status/{id}", request, true);
    }
}