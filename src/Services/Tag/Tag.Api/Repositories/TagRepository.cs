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
    public async Task CreateTag(TagBase tag) => await CreateAsync(tag);

    public async Task UpdateTag(TagBase tag) => await UpdateAsync(tag);

    public async Task DeleteTag(TagBase tag) => await DeleteAsync(tag);

    public async Task<IEnumerable<TagBase>> GetTags(int count) => await FindAll().Take(count).ToListAsync();

    public async Task<TagBase?> GetTagById(Guid id) => await GetByIdAsync(id) ?? null;
}