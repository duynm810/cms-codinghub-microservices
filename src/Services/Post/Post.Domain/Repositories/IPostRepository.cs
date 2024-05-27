using Contracts.Domains.Repositories;
using Post.Domain.Entities;
using Shared.Responses;

namespace Post.Domain.Repositories;

public interface IPostRepository : IRepositoryCommandBase<PostBase, Guid>
{
    #region CRUD

    Guid CreatePost(PostBase post);

    Task<PostBase> UpdatePost(PostBase post);

    Task DeletePost(PostBase post);

    Task<IEnumerable<PostBase>> GetPosts();

    Task<PostBase?> GetPostById(Guid id);

    #endregion

    #region OTHERS

    Task<PagedResponse<PostBase>> GetPostsPaging(int pageNumber = 1, int pageSize = 10);

    Task<bool> SlugExists(string slug, Guid? currentId = null);

    Task<bool> HasPostsInCategory(long categoryId);

    Task<IEnumerable<PostBase>> GetPostsByIds(Guid[] ids);
    
    Task<IEnumerable<PostBase>> GetFeaturedPosts(int count);

    Task ApprovePost(PostBase post);

    Task SubmitPostForApproval(PostBase post);

    Task RejectPostWithReason(PostBase post);

    #endregion
}