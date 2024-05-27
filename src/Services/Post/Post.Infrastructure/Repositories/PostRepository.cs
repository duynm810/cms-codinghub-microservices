using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Infrastructure.Paged;
using Microsoft.EntityFrameworkCore;
using Post.Domain.Entities;
using Post.Domain.Repositories;
using Post.Infrastructure.Persistence;
using Shared.Enums;
using Shared.Responses;

namespace Post.Infrastructure.Repositories;

public class PostRepository(PostContext dbContext, IUnitOfWork<PostContext> unitOfWork)
    : RepositoryCommandBase<PostBase, Guid, PostContext>(dbContext, unitOfWork), IPostRepository
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

    #region OTHERS

    public async Task<PagedResponse<PostBase>> GetPostsPaging(int pageNumber, int pageSize)
    {
        var query = FindAll();

        var items = await PagedList<PostBase>.ToPagedList(query, pageNumber, pageSize, x => x.CreatedDate);

        var response = new PagedResponse<PostBase>
        {
            Items = items,
            MetaData = items.GetMetaData()
        };

        return response;
    }

    public async Task<bool> SlugExists(string slug, Guid? currentId = null)
    {
        return await FindByCondition(x => x.Slug == slug && (!currentId.HasValue || x.Id != currentId.Value))
            .AnyAsync();
    }

    public async Task<bool> HasPostsInCategory(long categoryId)
    {
        return await FindByCondition(x => x.CategoryId == categoryId).AnyAsync();
    }

    public async Task<IEnumerable<PostBase>> GetPostsByIds(Guid[] ids)
    {
        return await FindByCondition(c => ids.Contains(c.Id)).ToListAsync();
    }

    public async Task<IEnumerable<PostBase>> GetFeaturedPosts(int count) =>
        await FindAll().OrderByDescending(x => x.ViewCount).Take(count).ToListAsync();

    public async Task ApprovePost(PostBase post)
    {
        post.Status = PostStatusEnum.Published;
        await UpdateAsync(post);
    }

    public async Task SubmitPostForApproval(PostBase post)
    {
        post.Status = PostStatusEnum.WaitingForApproval;
        await UpdateAsync(post);
    }

    public async Task RejectPostWithReason(PostBase post)
    {
        post.Status = PostStatusEnum.Rejected;
        await UpdateAsync(post);
    }

    #endregion
}