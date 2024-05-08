using Shared.Dtos.Category;

namespace Post.Domain.Interfaces;

public interface ICategoryGrpcService
{
    Task<CategoryDto?> GetCategoryById(long id);
}