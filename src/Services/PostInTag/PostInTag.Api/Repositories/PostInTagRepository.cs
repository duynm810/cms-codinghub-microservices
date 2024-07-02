using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using PostInTag.Api.Entities;
using PostInTag.Api.Persistence;
using PostInTag.Api.Repositories.Interfaces;

namespace PostInTag.Api.Repositories;

public class PostInTagRepository(PostInTagContext dbContext, IUnitOfWork<PostInTagContext> unitOfWork)
    : RepositoryCommandBase<PostInTagBase, Guid, PostInTagContext>(dbContext, unitOfWork), IPostInTagRepository
{
    public async Task CreatePostToTag(PostInTagBase postInTagBase) => await CreateAsync(postInTagBase);

    public async Task DeletePostToTag(PostInTagBase postInTagBase) => await DeleteAsync(postInTagBase);

    public async Task<PostInTagBase?> GetPostInTag(Guid postId, Guid tagId) =>
        await FindByCondition(p => p.PostId == postId && p.TagId == tagId).FirstOrDefaultAsync();

    public async Task DeleteTagsByPostId(Guid postId)
    {
        var tagsToDelete = await FindByCondition(p => p.PostId == postId).ToListAsync();
        if (tagsToDelete.Count != 0)
        {
            await DeleteListAsync(tagsToDelete);
        }
    }
}