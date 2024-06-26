using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using PostInTag.Grpc.Entities;
using PostInTag.Grpc.Persistence;
using PostInTag.Grpc.Repositories.Interfaces;

namespace PostInTag.Grpc.Repositories;

public class PostInTagRepository(PostInTagContext dbContext, IUnitOfWork<PostInTagContext> unitOfWork)
    : RepositoryCommandBase<PostInTagBase, Guid, PostInTagContext>(dbContext, unitOfWork), IPostInTagRepository
{
    public async Task<IEnumerable<Guid>> GetTagIdsByPostId(Guid postId) =>
        await FindByCondition(x => x.PostId == postId).Select(x => x.TagId).ToListAsync();

    public async Task<IEnumerable<Guid>> GetPostIdsInTag(Guid tagId) =>
        await FindByCondition(x => x.TagId == tagId).Select(x => x.PostId).ToListAsync();
}