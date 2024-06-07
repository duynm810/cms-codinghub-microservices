using Shared.Dtos.Category;

namespace PostInTag.Api.GrpcServices.Interfaces;

public interface ICategoryGrpcService
{
    Task<IEnumerable<CategoryDto>> GetCategoriesByIds(IEnumerable<long> ids);
}