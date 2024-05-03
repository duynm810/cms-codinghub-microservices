using Category.API.Entities;
using Contracts.Domains.Repositories;

namespace Category.API.Repositories.Interfaces;

public interface ICategoryRepository : IRepositoryCommandBase<CategoryBase, Guid>
{
    Task<IEnumerable<CategoryBase>> GetCategories();
}