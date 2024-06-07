using Shared.Dtos.Post;
using Shared.Dtos.PostInSeries;
using Shared.Responses;

namespace WebApps.UI.ApiServices.Interfaces;

public interface IPostApiClient
{
    Task<ApiResult<List<PostDto>>> GetFeaturedPosts(int count);
    Task<ApiResult<List<PostDto>>> GetPinnedPosts(int count);

    Task<ApiResult<PagedResponse<PostDto>>> GetPostsByCategoryPaging(string categorySlug, int pageNumber, int pageSize);

    Task<ApiResult<PagedResponse<PostDto>>> GetPostsByTagPaging(string tagSlug, int pageNumber, int pageSize);
    
    Task<ApiResult<PostDetailDto>> GetPostBySlug(string slug, int relatedCount);

    Task<ApiResult<PagedResponse<PostDto>>> GetLatestPostsPaging(int pageNumber, int pageSize);

    Task<ApiResult<PagedResponse<PostDto>>> SearchPostsPaging(string keyword, int pageNumber, int pageSize);

    Task<ApiResult<PagedResponse<PostInSeriesDto>>> GetPostsInSeriesBySlugPaging(string seriesSlug, int pageNumber,
        int pageSize);

    Task<ApiResult<List<PostDto>>> GetMostCommentedPosts(int count);

    Task<ApiResult<List<PostDto>>> GetMostLikedPosts(int count);

    Task<ApiResult<List<PostsByNonStaticPageCategoryDto>>> GetPostsByNonStaticPageCategory(int count);
}