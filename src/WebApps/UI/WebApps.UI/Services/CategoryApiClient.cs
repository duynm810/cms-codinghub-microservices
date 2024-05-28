using Shared.Dtos.Category;
using Shared.Responses;
using WebApps.UI.Services.Interfaces;

namespace WebApps.UI.Services;

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