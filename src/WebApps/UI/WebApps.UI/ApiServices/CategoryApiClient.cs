using Shared.Dtos.Category;
using Shared.Responses;
using WebApps.UI.ApiServices.Interfaces;

namespace WebApps.UI.ApiServices;

public class CategoryApiClient(IBaseApiClient baseApiClient) : ICategoryApiClient
{
    public async Task<ApiResult<List<CategoryDto>>> GetCategories()
    {
        return await baseApiClient.GetListAsync<CategoryDto>("/categories");
    }

    public async Task<ApiResult<CategoryDto>> GetCategoryBySlug(string slug)
    {
        return await baseApiClient.GetAsync<CategoryDto>($"/categories/by-slug/{slug}");
    }
}