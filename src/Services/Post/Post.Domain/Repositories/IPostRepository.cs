using Contracts.Domains.Repositories;
using Post.Domain.Entities;
using Shared.Dtos.Identity.User;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Domain.Repositories;

public interface IPostRepository : IRepositoryCommandBase<PostBase, Guid>
{
    #region CRUD

    Guid CreatePost(PostBase post);

    Task<bool> UpdatePost(PostBase post);

    Task DeletePost(PostBase post);

    Task<IEnumerable<PostBase>> GetPosts();

    Task<PostBase?> GetPostById(Guid id);

    #endregion

    #region OTHERS

    Task<PagedResponse<PostBase>> GetPostsPaging(string? filter, int pageNumber, int pageSize);

    Task<PagedResponse<PostBase>> GetPostsByCategoryPaging(long categoryId, int pageNumber, int pageSize);
    
    Task<PagedResponse<PostBase>> GetPostsByAuthorPaging(Guid authorId, int pageNumber, int pageSize);
    
    Task<PagedResponse<PostBase>> GetPostsByCurrentUserPaging(SearchPostByCurrentUserDto filter, CurrentUserDto currentUser, int pageNumber, int pageSize);
    
    Task<PagedResponse<PostBase>> GetLatestPostsPaging(int pageNumber, int pageSize);

    Task<IEnumerable<PostBase>> GetPostsByCategoryId(long categoryId, int count);

    Task<PostBase?> GetPostBySlug(string slug);

    Task<IEnumerable<PostBase>> GetPostsByIds(Guid[] ids);

    Task<IEnumerable<PostBase>> GetRelatedPosts(PostBase post, int count);

    Task<IEnumerable<PostBase>> GetFeaturedPosts(int count);

    Task<IEnumerable<PostBase>> GetPinnedPosts(int count);

    Task<IEnumerable<PostBase>> GetMostCommentPosts(int count);

    Task<IEnumerable<PostBase>> GetMostLikedPosts(int count);

    Task<IEnumerable<PostBase>> GetTop10Posts();

    Task<bool> SlugExists(string slug, Guid? currentId = null);

    Task<bool> HasPostsInCategory(long categoryId);

    Task ApprovePost(PostBase post);

    Task SubmitPostForApproval(PostBase post);

    Task RejectPostWithReason(PostBase post);

    Task<bool> TogglePinStatus(PostBase post, bool isPinned);
    
    Task<bool> ToggleFeaturedStatus(PostBase post, bool isFeatured);

    #endregion
}