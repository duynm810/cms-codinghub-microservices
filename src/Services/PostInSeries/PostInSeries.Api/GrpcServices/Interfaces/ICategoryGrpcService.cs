using Shared.Dtos.Category;

namespace PostInSeries.Api.GrpcServices.Interfaces;

public interface ICategoryGrpcService
{
    Task<IEnumerable<CategoryDto>> GetCategoriesByIds(IEnumerable<long> ids);
}