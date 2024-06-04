using Category.Api.Entities;
using Contracts.Domains.Repositories;
using Shared.Responses;

namespace Category.Api.Repositories.Interfaces;

public interface ICategoryRepository : IRepositoryCommandBase<CategoryBase, long>
{
    #region CRUD

    Task CreateCategory(CategoryBase category);

    Task UpdateCategory(CategoryBase category);

    Task DeleteCategory(CategoryBase category);

    Task<IEnumerable<CategoryBase>> GetCategories();

    Task<CategoryBase?> GetCategoryById(long id);

    #endregion

    #region OTHERS

    Task<PagedResponse<CategoryBase>> GetCategoriesPaging(int pageNumber = 1, int pageSize = 10);

    Task<CategoryBase?> GetCategoryBySlug(string slug);

    #endregion
}