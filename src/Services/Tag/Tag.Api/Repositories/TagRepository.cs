using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using Tag.Api.Entities;
using Tag.Api.Persistence;
using Tag.Api.Repositories.Interfaces;

namespace Tag.Api.Repositories;

public class TagRepository(TagContext dbContext, IUnitOfWork<TagContext> unitOfWork)
    : RepositoryCommandBase<TagBase, Guid, TagContext>(dbContext, unitOfWork), ITagRepository
{
    #region CRUD

    public async Task CreateTag(TagBase tag) => await CreateAsync(tag);

    public async Task UpdateTag(TagBase tag) => await UpdateAsync(tag);

    public async Task DeleteTag(TagBase tag) => await DeleteAsync(tag);

    public async Task<IEnumerable<TagBase>> GetTags(int count) => await FindAll().Take(count).ToListAsync();

    public async Task<TagBase?> GetTagById(Guid id) => await GetByIdAsync(id) ?? null;

    #endregion

    #region OTHERS

    public async Task<TagBase?> GetTagBySlug(string slug) => await FindByCondition(x => x.Slug == slug).FirstAsync();

    public async Task<TagBase?> GetTagByName(string name) =>
        await FindByCondition(x => x.Name == name).FirstOrDefaultAsync();

    public async Task<IEnumerable<TagBase>> GetSuggestedTags(string? keyword, int count)
    {
        var query = FindAll();
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name.Contains(keyword));
        }
        
        var tags = await query.OrderByDescending(x => x.UsageCount)
            .Take(count)
            .ToListAsync();

        return tags;
    }

    #endregion
}