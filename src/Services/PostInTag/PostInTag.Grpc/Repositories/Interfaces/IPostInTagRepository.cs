using Contracts.Domains.Repositories;
using PostInTag.Grpc.Entities;

namespace PostInTag.Grpc.Repositories.Interfaces;

public interface IPostInTagRepository : IRepositoryCommandBase<PostInTagBase, Guid>
{
    Task<IEnumerable<Guid>> GetTagIdsByPostId(Guid postId);
}