using Category.GRPC.Entities;
using Category.GRPC.Persistence;
using Contracts.Domains.Repositories;

namespace Category.GRPC.Repositories.Interfaces;

public interface ICategoryRepository : IRepositoryQueryBase<CategoryBase, long, CategoryContext>
{
    Task<bool> HasPost(long categoryId);
}