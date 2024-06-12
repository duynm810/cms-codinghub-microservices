using Shared.Dtos.Category;

namespace PostInSeries.Api.GrpcClients.Interfaces;

public interface ICategoryGrpcClient
{
    Task<IEnumerable<CategoryDto>> GetCategoriesByIds(IEnumerable<long> ids);
}