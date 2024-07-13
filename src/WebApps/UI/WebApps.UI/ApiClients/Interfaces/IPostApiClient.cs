using Shared.Dtos.Post;
using Shared.Requests.Post.Commands;
using Shared.Requests.Post.Queries;
using Shared.Responses;

namespace WebApps.UI.ApiClients.Interfaces;

public interface IPostApiClient
{
    Task<ApiResult<Guid>> CreatePost(CreatePostRequest request);

    Task<ApiResult<bool>> UpdatePost(Guid id, UpdatePostRequest request);

    Task<ApiResult<bool>> UpdateThumbnail(Guid id, UpdateThumbnailRequest request);
    
    Task<ApiResult<bool>> DeletePost(Guid id);

    Task<ApiResult<bool>> ApprovePost(Guid id);

    Task<ApiResult<bool>> SubmitPostForApproval(Guid id);

    Task<ApiResult<bool>> RejectPostWithReason(Guid id, RejectPostWithReasonRequest request);
    
    Task<ApiResult<PostDto>> GetPostBySlug(string slug);

    Task<ApiResult<PostsByCategoryDto>> GetPostsByCategoryPaging(string categorySlug, GetPostsByCategoryRequest request);

    Task<ApiResult<PostsBySeriesDto>> GetPostsBySeriesPaging(string seriesSlug, GetPostsBySeriesRequest request);
    
    Task<ApiResult<PostsByTagDto>> GetPostsByTagPaging(string tagSlug, GetPostsByTagRequest request);
    
    Task<ApiResult<PostsByAuthorDto>> GetPostsByAuthorPaging(string userName, GetPostsByAuthorRequest request);

    Task<ApiResult<PagedResponse<PostDto>>> GetPostsByCurrentUserPaging(GetPostsByCurrentUserRequest request);
    
    Task<ApiResult<PagedResponse<PostDto>>> GetLatestPostsPaging(GetLatestPostsRequest request);
    
    Task<ApiResult<PostsBySlugDto>> GetDetailBySlug(string slug, int relatedCount);

    Task<ApiResult<PagedResponse<PostDto>>> SearchPostsPaging(GetPostsRequest request);

    Task<ApiResult<List<PostDto>>> GetMostCommentedPosts(int count);

    Task<ApiResult<List<PostsByNonStaticPageCategoryDto>>> GetPostsByNonStaticPageCategory(int count);

    Task<ApiResult<bool>> TogglePinStatus(Guid id, TogglePinStatusRequest request);

    Task<ApiResult<bool>> ToggleFeaturedStatus(Guid id, ToggleFeaturedStatusRequest request);
}