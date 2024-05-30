using Category.Grpc.Entities;
using Category.Grpc.Persistence;
using Category.Grpc.Repositories.Interfaces;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Category.Grpc.Repositories;

public class CategoryRepository(CategoryContext dbContext)
    : RepositoryQueryBase<CategoryBase, long, CategoryContext>(dbContext), ICategoryRepository
{
    public async Task<CategoryBase?> GetCategoryById(long id) => await GetByIdAsync(id);

    public async Task<IEnumerable<CategoryBase>> GetCategoriesByIds(long[] ids)
    {
        var categories = await FindByCondition(c => ids.Contains(c.Id)).ToListAsync();
        return categories;
    }

    public async Task<CategoryBase?> GetCategoryBySlug(string slug) =>
        await FindByCondition(x => x.Slug == slug).FirstOrDefaultAsync();
}