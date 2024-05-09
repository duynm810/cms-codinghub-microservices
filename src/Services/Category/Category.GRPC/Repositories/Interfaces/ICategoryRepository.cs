using Category.GRPC.Entities;
using Category.GRPC.Persistence;
using Contracts.Domains.Repositories;

namespace Category.GRPC.Repositories.Interfaces;

public interface ICategoryRepository : IRepositoryQueryBase<CategoryBase, long, CategoryContext>
{
    Task<CategoryBase?> GetCategoryById(long id);

    Task<IEnumerable<CategoryBase>> GetCategoryByIds(long[] ids);
}