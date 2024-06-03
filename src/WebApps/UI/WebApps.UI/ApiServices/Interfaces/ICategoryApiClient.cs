using Shared.Dtos.Category;
using Shared.Responses;

namespace WebApps.UI.ApiServices.Interfaces;

public interface ICategoryApiClient
{
    Task<ApiResult<List<CategoryDto>>> GetCategories();
    
    Task<ApiResult<CategoryDto>> GetCategoryBySlug(string slug);
}