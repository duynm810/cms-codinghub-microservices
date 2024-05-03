using Shared.Dtos.Category;
using Shared.Responses;

namespace Category.API.Services.Interfaces;

public interface ICategoryService
{
    Task<ApiResult<IEnumerable<CategoryDto>>> GetCategories();
}