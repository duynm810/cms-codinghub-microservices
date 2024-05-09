using Shared.Dtos.Category;
using Shared.Responses;

namespace Category.API.Services.Interfaces;

public interface ICategoryService
{
    #region CRUD

    Task<ApiResult<CategoryDto>> CreateCategory(CreateCategoryDto model);

    Task<ApiResult<CategoryDto>> UpdateCategory(long id, UpdateCategoryDto model);

    Task<ApiResult<bool>> DeleteCategory(long[] ids);

    Task<ApiResult<IEnumerable<CategoryDto>>> GetCategories();

    Task<ApiResult<CategoryDto>> GetCategoryById(long id);

    #endregion

    #region OTHERS

    Task<ApiResult<PagedResponse<CategoryDto>>> GetCategoriesPaging(int pageNumber = 1, int pageSize = 10);

    #endregion
}