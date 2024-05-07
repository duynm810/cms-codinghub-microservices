using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using Post.Domain.Entities;
using Post.Domain.Interfaces;
using Post.Infrastructure.Persistence;

namespace Post.Infrastructure.Repositories;

public class PostRepository(PostContext dbContext, IUnitOfWork<PostContext> unitOfWork) 
    : RepositoryCommandBase<PostBase, Guid, PostContext>(dbContext, unitOfWork),  IPostRepository
{
    #region CRUD

    public Guid CreatePost(PostBase post)
    {
        Create(post);
        return post.Id;
    }

    public async Task<PostBase> UpdatePost(PostBase post)
    {
        await UpdateAsync(post);
        return post;
    }

    public async Task DeletePost(PostBase post) => await DeleteAsync(post);

    public async Task<IEnumerable<PostBase>> GetPosts() => await FindAll().ToListAsync();

    public async Task<PostBase?> GetPostById(Guid id) => await GetByIdAsync(id) ?? null;

    #endregion
}