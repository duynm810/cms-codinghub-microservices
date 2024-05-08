using Category.GRPC.Entities;
using Category.GRPC.Persistence;
using Category.GRPC.Repositories.Interfaces;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Category.GRPC.Repositories;

public class CategoryRepository(CategoryContext dbContext) :  RepositoryQueryBase<CategoryBase, long, CategoryContext>(dbContext), ICategoryRepository
{
    public async Task<bool> HasPost(long categoryId)
    {
        return await FindByCondition(x => x.Id == categoryId).AnyAsync();
    }
}