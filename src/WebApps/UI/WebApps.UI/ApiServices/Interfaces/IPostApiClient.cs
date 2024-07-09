using Shared.Dtos.Post.Commands;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace WebApps.UI.ApiServices.Interfaces;

public interface IPostApiClient
{
    Task<ApiResult<Guid>> CreatePost(CreatePostDto request);

    Task<ApiResult<bool>> UpdatePost(Guid id, UpdatePostDto request);

    Task<ApiResult<bool>> UpdateThumbnail(Guid id, UpdateThumbnailDto request);
    
    Task<ApiResult<bool>> DeletePost(Guid id);
    
    Task<ApiResult<PostDto>> GetPostBySlug(string slug);

    Task<ApiResult<PostsByCategoryDto>> GetPostsByCategoryPaging(string categorySlug, int pageNumber, int pageSize);

    Task<ApiResult<PostsBySeriesDto>> GetPostsBySeriesPaging(string seriesSlug, int pageNumber, int pageSize);
    
    Task<ApiResult<PostsByTagDto>> GetPostsByTagPaging(string tagSlug, int pageNumber, int pageSize);
    
    Task<ApiResult<PostsByAuthorDto>> GetPostsByAuthorPaging(string userName, int pageNumber, int pageSize);

    Task<ApiResult<PagedResponse<PostDto>>> GetPostsByCurrentUserPaging(int pageNumber, int pageSize);
    
    Task<ApiResult<PagedResponse<PostDto>>> GetLatestPostsPaging(int pageNumber, int pageSize);
    
    Task<ApiResult<PostsBySlugDto>> GetDetailBySlug(string slug, int relatedCount);

    Task<ApiResult<PagedResponse<PostDto>>> SearchPostsPaging(string keyword, int pageNumber, int pageSize);

    Task<ApiResult<List<PostDto>>> GetMostCommentedPosts(int count);

    Task<ApiResult<List<PostsByNonStaticPageCategoryDto>>> GetPostsByNonStaticPageCategory(int count);

    Task<ApiResult<bool>> TogglePinStatus(Guid id, TogglePinStatusDto request);

    Task<ApiResult<bool>> ToggleFeaturedStatus(Guid id, ToggleFeaturedStatusDto request);
}