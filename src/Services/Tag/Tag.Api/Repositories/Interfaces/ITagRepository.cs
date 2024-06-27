using Contracts.Domains.Repositories;
using Tag.Api.Entities;

namespace Tag.Api.Repositories.Interfaces;

public interface ITagRepository : IRepositoryCommandBase<TagBase, Guid>
{
    #region CRUD

    Task CreateTag(TagBase tag);

    Task UpdateTag(TagBase tag);

    Task DeleteTag(TagBase tag);

    Task<IEnumerable<TagBase>> GetTags(int count);

    Task<TagBase?> GetTagById(Guid id);

    #endregion

    #region OTHERS

    Task<TagBase?> GetTagBySlug(string slug);

    Task<TagBase?> GetTagByName(string name);

    Task<IEnumerable<TagBase>> GetSuggestedTags(string? keyword, int count);

    #endregion
}