using Category.GRPC.Entities;
using Category.GRPC.Persistence;
using Category.GRPC.Repositories.Interfaces;
using Infrastructure.Domains.Repositories;

namespace Category.GRPC.Repositories;

public class CategoryRepository(CategoryContext dbContext)
    : RepositoryQueryBase<CategoryBase, long, CategoryContext>(dbContext), ICategoryRepository
{
    public async Task<CategoryBase?> GetCategoryById(long id) => await GetByIdAsync(id);
}