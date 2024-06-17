using Shared.Dtos.Category;

namespace PostInTag.Api.GrpcClients.Interfaces;

public interface ICategoryGrpcClient
{
    Task<IEnumerable<CategoryDto>> GetCategoriesByIds(IEnumerable<long> ids);
}