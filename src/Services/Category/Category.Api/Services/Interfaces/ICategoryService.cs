using Shared.Dtos.Category;
using Shared.Requests.Category;
using Shared.Responses;

namespace Category.Api.Services.Interfaces;

public interface ICategoryService
{
    #region CRUD

    Task<ApiResult<CategoryDto>> CreateCategory(CreateCategoryRequest request);

    Task<ApiResult<CategoryDto>> UpdateCategory(long id, UpdateCategoryRequest request);

    Task<ApiResult<bool>> DeleteCategory(List<long> ids);

    Task<ApiResult<IEnumerable<CategoryDto>>> GetCategories();

    Task<ApiResult<CategoryDto>> GetCategoryById(long id);

    #endregion

    #region OTHERS

    Task<ApiResult<PagedResponse<CategoryDto>>> GetCategoriesPaging(GetCategoriesRequest request);

    Task<ApiResult<CategoryDto>> GetCategoryBySlug(string slug);

    #endregion
}