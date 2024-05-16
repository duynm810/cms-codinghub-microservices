using Contracts.Domains.Repositories;
using Post.Domain.Entities;

namespace Post.Domain.Repositories;

public interface IPostActivityLogRepository : IRepositoryCommandBase<PostActivityLog, Guid>
{
    Task CreatePostActivityLogs(PostActivityLog postActivityLog);
    
    Task<IEnumerable<PostActivityLog>> GetActivityLogs(Guid postId);
}