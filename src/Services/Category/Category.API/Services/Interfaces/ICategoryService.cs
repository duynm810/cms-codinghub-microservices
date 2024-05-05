using Shared.Dtos.Category;
using Shared.Responses;

namespace Category.API.Services.Interfaces;

public interface ICategoryService
{
    #region CRUD

    Task<ApiResult<CategoryDto>> CreateCategory(CreateCategoryDto model);

    Task<ApiResult<CategoryDto>> UpdateCategory(Guid id, UpdateCategoryDto model);

    Task<ApiResult<CategoryDto>> DeleteCategory(Guid[] ids);
    
    Task<ApiResult<IEnumerable<CategoryDto>>> GetCategories();
    
    Task<ApiResult<CategoryDto>> GetCategoryById(Guid id);

    #endregion
    
}