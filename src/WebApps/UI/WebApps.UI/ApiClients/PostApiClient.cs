using Shared.Dtos.Post;
using Shared.Requests.Post.Commands;
using Shared.Requests.Post.Queries;
using Shared.Responses;
using WebApps.UI.ApiClients.Interfaces;

namespace WebApps.UI.ApiClients;

public class PostApiClient(IBaseApiClient baseApiClient) : IPostApiClient
{
    public async Task<ApiResult<Guid>> CreatePost(CreatePostRequest request)
    {
        return await baseApiClient.PostAsync<CreatePostRequest, Guid>($"/posts", request, true);
    }

    public async Task<ApiResult<bool>> UpdatePost(Guid id, UpdatePostRequest request)
    {
        return await baseApiClient.PutAsync<UpdatePostRequest, bool>($"/posts/{id}", request, true);
    }
    
    public async Task<ApiResult<bool>> UpdateThumbnail(Guid id, UpdateThumbnailRequest request)
    {
        return await baseApiClient.PutAsync<UpdateThumbnailRequest, bool>($"/posts/update-thumbnail/{id}", request, true);
    }

    public async Task<ApiResult<bool>> DeletePost(Guid id)
    {
        return await baseApiClient.DeleteAsync<bool>($"/posts/{id}", true);
    }
    
    public async Task<ApiResult<bool>> ApprovePost(Guid id, ApprovePostRequest request)
    {
        return await baseApiClient.PostAsync<ApprovePostRequest, bool>($"/posts/approve/{id}", request, true);
    }

    public async Task<ApiResult<bool>> SubmitPostForApproval(Guid id, SubmitPostForApprovalRequest request)
    {
        return await baseApiClient.PostAsync<SubmitPostForApprovalRequest, bool>($"/posts/submit-for-approval/{id}", request, true);
    }
    
    public async Task<ApiResult<bool>> RejectPostWithReason(Guid id, RejectPostWithReasonRequest request)
    {
        return await baseApiClient.PostAsync<RejectPostWithReasonRequest, bool>($"/posts/reject/{id}", request, true);
    }
    
    public async Task<ApiResult<PostDto>> GetPostBySlug(string slug)
    {
        return await baseApiClient.GetAsync<PostDto>($"/posts/slug/{slug}", true);
    }

    public async Task<ApiResult<PostsByCategoryDto>> GetPostsByCategoryPaging(string categorySlug, GetPostsByCategoryRequest request)
    {
        return await baseApiClient.PostAsync<GetPostsByCategoryRequest, PostsByCategoryDto>($"/posts/by-category/{categorySlug}/paging", request);
    }
    
    public async Task<ApiResult<PostsBySeriesDto>> GetPostsBySeriesPaging(string seriesSlug, GetPostsBySeriesRequest request)
    {
        return await baseApiClient.PostAsync<GetPostsBySeriesRequest, PostsBySeriesDto>($"/posts/by-series/{seriesSlug}/paging", request);
    }

    public async Task<ApiResult<PostsByTagDto>> GetPostsByTagPaging(string tagSlug, GetPostsByTagRequest request)
    {
        return await baseApiClient.PostAsync<GetPostsByTagRequest, PostsByTagDto>($"/posts/by-tag/{tagSlug}/paging", request);
    }
    
    public async Task<ApiResult<PostsByAuthorDto>> GetPostsByAuthorPaging(string userName, GetPostsByAuthorRequest request)
    {
        return await baseApiClient.PostAsync<GetPostsByAuthorRequest, PostsByAuthorDto>($"/posts/by-author/{userName}/paging", request);
    }
    
    public async Task<ApiResult<PagedResponse<PostDto>>> GetPostsByCurrentUserPaging(GetPostsByCurrentUserRequest request)
    {
        return await baseApiClient.PostAsync<GetPostsByCurrentUserRequest, PagedResponse<PostDto>>($"/posts/by-current-user/paging", request, true);
    }
    
    public async Task<ApiResult<PagedResponse<PostDto>>> GetLatestPostsPaging(GetLatestPostsRequest request)
    {
        return await baseApiClient.PostAsync<GetLatestPostsRequest, PagedResponse<PostDto>>("/posts/latest/paging", request);
    }
    
    public async Task<ApiResult<PostsBySlugDto>> GetDetailBySlug(string slug, int relatedCount)
    {
        return await baseApiClient.GetAsync<PostsBySlugDto>($"/posts/detail/by-slug/{slug}?relatedCount={relatedCount}");
    }

    public async Task<ApiResult<PagedResponse<PostDto>>> SearchPostsPaging(GetPostsRequest request)
    {
        return await baseApiClient.PostAsync<GetPostsRequest, PagedResponse<PostDto>>($"/posts/paging", request);
    }
    
    public async Task<ApiResult<List<PostDto>>> GetMostCommentedPosts(int count)
    {
        return await baseApiClient.GetListAsync<PostDto>($"/posts/most-commented?count={count}");
    }

    public async Task<ApiResult<List<PostsByNonStaticPageCategoryDto>>> GetPostsByNonStaticPageCategory(int count)
    {
        return await baseApiClient.GetListAsync<PostsByNonStaticPageCategoryDto>($"/posts/by-non-static-page-category?count={count}");
    }
    
    public async Task<ApiResult<bool>> TogglePinStatus(Guid id, TogglePinStatusRequest request)
    {
        return await baseApiClient.PutAsync<TogglePinStatusRequest, bool>($"/posts/toggle-pin-status/{id}", request, true);
    }
    
    public async Task<ApiResult<bool>> ToggleFeaturedStatus(Guid id, ToggleFeaturedStatusRequest request)
    {
        return await baseApiClient.PutAsync<ToggleFeaturedStatusRequest, bool>($"/posts/toggle-featured-status/{id}", request, true);
    }
}