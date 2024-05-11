using Contracts.Domains.Repositories;
using Post.Domain.Entities;

namespace Post.Domain.Repositories;

public interface IPostActivityLogRepository : IRepositoryQueryBase<PostActivityLog, Guid>
{
    Task<IEnumerable<PostActivityLog>> GetActivityLogs(Guid postId);
}