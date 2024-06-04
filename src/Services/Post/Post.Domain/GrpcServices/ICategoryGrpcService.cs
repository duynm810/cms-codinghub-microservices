using Shared.Dtos.Category;

namespace Post.Domain.GrpcServices;

public interface ICategoryGrpcService
{
    Task<CategoryDto?> GetCategoryById(long id);

    Task<IEnumerable<CategoryDto>> GetCategoriesByIds(IEnumerable<long> ids);

    Task<CategoryDto?> GetCategoryBySlug(string slug);

    Task<IEnumerable<CategoryDto?>> GetAllNonStaticPageCategories();
}