using Shared.Dtos.Category;

namespace Post.Domain.GrpcClients;

public interface ICategoryGrpcClient
{
    Task<CategoryDto?> GetCategoryById(long id);

    Task<IEnumerable<CategoryDto>> GetCategoriesByIds(IEnumerable<long> ids);

    Task<CategoryDto?> GetCategoryBySlug(string slug);

    Task<IEnumerable<CategoryDto>> GetAllNonStaticPageCategories();
}