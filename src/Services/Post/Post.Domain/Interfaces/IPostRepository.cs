using Contracts.Domains.Repositories;
using Post.Domain.Entities;
using Shared.Responses;

namespace Post.Domain.Interfaces;

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

    Task<PagedResponse<PostBase>> GetPostsPaging(int pageNumber, int pageSize);

    #endregion
}