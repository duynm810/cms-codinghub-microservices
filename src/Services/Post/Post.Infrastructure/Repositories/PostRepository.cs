using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using Post.Domain.Entities;
using Post.Infrastructure.Persistence;
using Post.Infrastructure.Repositories.Interfaces;

namespace Post.Infrastructure.Repositories;

public class PostRepository(PostContext dbContext, IUnitOfWork<PostContext> unitOfWork) 
    : RepositoryCommandBase<PostBase, Guid, PostContext>(dbContext, unitOfWork),  IPostRepository
{
    #region CRUD

    public async Task CreatePost(PostBase post) => await CreateAsync(post);

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