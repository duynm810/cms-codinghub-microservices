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

    public async Task<IEnumerable<Guid>?> GetPostIdsInTag(Guid tagId) =>
        await FindByCondition(x => x.TagId == tagId).Select(x => x.PostId).ToListAsync();
}