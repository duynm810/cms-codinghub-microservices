using Shared.Dtos.Post;
using Shared.Dtos.PostInSeries;
using Shared.Responses;
using WebApps.UI.Models.Posts;

namespace WebApps.UI.Services.Interfaces;

public interface IPostApiClient
{
    Task<ApiResult<List<PostDto>>> GetFeaturedPosts();

    Task<ApiResult<PagedResponse<PostDto>>> GetPostsByCategoryPaging(string categorySlug, int pageNumber, int pageSize);
    
    Task<ApiResult<PostDetailDto>> GetPostBySlug(string slug);

    Task<ApiResult<PagedResponse<PostDto>>> GetLatestPostsPaging(int pageNumber, int pageSize);

    Task<ApiResult<PagedResponse<PostDto>>> SearchPostsPaging(string keyword, int pageNumber, int pageSize);

    Task<ApiResult<PagedResponse<PostInSeriesDto>>> GetPostsInSeriesBySlugPaging(string seriesSlug, int pageNumber, int pageSize);

    Task<ApiResult<List<PostDto>>> GetMostCommentedPosts();

    Task<ApiResult<List<PostDto>>> GetMostLikedPosts();

    Task<ApiResult<List<PostsByNonStaticPageCategoryDto>>> GetPostsByNonStaticPageCategory();
}