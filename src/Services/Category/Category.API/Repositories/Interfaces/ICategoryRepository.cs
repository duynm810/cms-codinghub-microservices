using Category.API.Entities;
using Contracts.Domains.Repositories;
using Shared.Responses;

namespace Category.API.Repositories.Interfaces;

public interface ICategoryRepository : IRepositoryCommandBase<CategoryBase, Guid>
{
    #region CRUD

    Task CreateCategory(CategoryBase category);
    
    Task UpdateCategory(CategoryBase category);
    
    Task DeleteCategory(CategoryBase category);
    
    Task<IEnumerable<CategoryBase>> GetCategories();

    Task<CategoryBase?> GetCategoryById(Guid id);

    #endregion

    #region OTHERS
    
    Task<PagedResponse<CategoryBase>> GetCategoriesPaging(int pageNumber, int pageSize);

    #endregion
}