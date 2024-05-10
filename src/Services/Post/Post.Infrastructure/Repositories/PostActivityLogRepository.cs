using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using Post.Domain.Entities;
using Post.Domain.Repositories;
using Post.Infrastructure.Persistence;

namespace Post.Infrastructure.Repositories;

public class PostActivityLogRepository(PostContext dbContext)
    : RepositoryQueryBase<PostActivityLog, Guid, PostContext>(dbContext), IPostActivityLogRepository
{
    public async Task<IEnumerable<PostActivityLog>> GetActivityLogs(Guid postId) =>
        await FindByCondition(x => x.PostId == postId).OrderByDescending(x => x.CreatedDate).ToListAsync();
}