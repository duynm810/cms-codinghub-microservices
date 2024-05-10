using Shared.Dtos.Category;
using Shared.Responses;

namespace Category.Api.Services.Interfaces;

public interface ICategoryService
{
    #region CRUD

    Task<ApiResult<CategoryDto>> CreateCategory(CreateCategoryDto model);

    Task<ApiResult<CategoryDto>> UpdateCategory(long id, UpdateCategoryDto model);

    Task<ApiResult<bool>> DeleteCategory(List<long> ids);

    Task<ApiResult<IEnumerable<CategoryDto>>> GetCategories();

    Task<ApiResult<CategoryDto>> GetCategoryById(long id);

    #endregion

    #region OTHERS

    Task<ApiResult<PagedResponse<CategoryDto>>> GetCategoriesPaging(int pageNumber, int pageSize);

    #endregion
}