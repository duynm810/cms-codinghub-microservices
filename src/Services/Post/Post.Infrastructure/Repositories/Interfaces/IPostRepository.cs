using Contracts.Domains.Repositories;
using Post.Domain.Entities;

namespace Post.Infrastructure.Repositories.Interfaces;

public interface IPostRepository : IRepositoryCommandBase<PostBase, Guid>
{
    #region CRUD

    Task CreatePost(PostBase post);
    
    Task<PostBase> UpdatePost(PostBase post);
    
    Task DeletePost(PostBase post);
    
    Task<IEnumerable<PostBase>> GetPosts();

    Task<PostBase?> GetPostById(Guid id);

    #endregion
}