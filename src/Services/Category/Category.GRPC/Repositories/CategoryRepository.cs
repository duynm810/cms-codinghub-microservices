using Category.GRPC.Entities;
using Category.GRPC.Persistence;
using Category.GRPC.Repositories.Interfaces;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Category.GRPC.Repositories;

public class CategoryRepository(CategoryContext dbContext)
    : RepositoryQueryBase<CategoryBase, long, CategoryContext>(dbContext), ICategoryRepository
{
    public async Task<CategoryBase?> GetCategoryById(long id) => await GetByIdAsync(id);

    public async Task<IEnumerable<CategoryBase>> GetCategoryByIds(long[] ids)
    {
        var categories = await FindByCondition(c => ids.Contains(c.Id)).Select(c => new CategoryBase()
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug
            })
            .ToListAsync();

        return categories;
    }
}