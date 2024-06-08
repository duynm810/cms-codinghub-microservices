using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using Tag.Grpc.Entities;
using Tag.Grpc.Persistence;
using Tag.Grpc.Repositories.Interfaces;

namespace Tag.Grpc.Repositories;

public class TagRepository(TagContext dbContext)
    : RepositoryQueryBase<TagBase, Guid, TagContext>(dbContext), ITagRepository
{
    public async Task<IEnumerable<TagBase>> GetTagsByIds(Guid[] ids) =>
        await FindByCondition(c => ids.Contains(c.Id)).ToListAsync();

    public async Task<IEnumerable<TagBase>> GetTags() => await FindAll().ToListAsync();
    
    public async Task<TagBase?> GetTagBySlug(string slug) =>
        await FindByCondition(x => x.Slug == slug).FirstOrDefaultAsync();
}