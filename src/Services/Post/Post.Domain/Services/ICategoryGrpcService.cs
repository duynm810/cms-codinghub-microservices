using Shared.Dtos.Category;

namespace Post.Domain.Services;

public interface ICategoryGrpcService
{
    Task<CategoryDto?> GetCategoryById(long id);
    
    Task<IEnumerable<CategoryDto>> GetCategoriesByIds(IEnumerable<long> ids);
}