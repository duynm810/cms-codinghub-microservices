using Category.Grpc.Entities;
using Category.Grpc.Persistence;
using Contracts.Domains.Repositories;

namespace Category.Grpc.Repositories.Interfaces;

public interface ICategoryRepository : IRepositoryQueryBase<CategoryBase, long, CategoryContext>
{
    Task<CategoryBase?> GetCategoryById(long id);

    Task<IEnumerable<CategoryBase>> GetCategoriesByIds(long[] ids);

    Task<CategoryBase?> GetCategoryBySlug(string slug);

    Task<IEnumerable<CategoryBase?>> GetAllNonStaticPageCategories();
}