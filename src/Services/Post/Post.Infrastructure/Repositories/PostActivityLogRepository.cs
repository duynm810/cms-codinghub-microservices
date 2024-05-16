using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using Post.Domain.Entities;
using Post.Domain.Repositories;
using Post.Infrastructure.Persistence;

namespace Post.Infrastructure.Repositories;

public class PostActivityLogRepository(PostContext dbContext, IUnitOfWork<PostContext> unitOfWork)
    : RepositoryCommandBase<PostActivityLog, Guid, PostContext>(dbContext, unitOfWork), IPostActivityLogRepository
{
    public async Task CreatePostActivityLogs(PostActivityLog postActivityLog) => await CreateAsync(postActivityLog);

    public async Task<IEnumerable<PostActivityLog>> GetActivityLogs(Guid postId) =>
        await FindByCondition(x => x.PostId == postId).OrderByDescending(x => x.CreatedDate).ToListAsync();
}