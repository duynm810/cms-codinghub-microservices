using Contracts.Domains.Repositories;
using Tag.Grpc.Entities;
using Tag.Grpc.Persistence;

namespace Tag.Grpc.Repositories.Interfaces;

public interface ITagRepository : IRepositoryQueryBase<TagBase, Guid, TagContext>
{
    Task<IEnumerable<TagBase>> GetTagsByIds(Guid[] ids);

    Task<IEnumerable<TagBase>> GetTags();

    Task<TagBase?> GetTagBySlug(string slug);
}