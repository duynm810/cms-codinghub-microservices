using Category.API.Entities;
using Category.API.Persistence;
using Category.API.Repositories.Interfaces;
using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Category.API.Repositories;

public class CategoryRepository(CategoryContext dbContext, IUnitOfWork<CategoryContext> unitOfWork) 
    : RepositoryCommandBase<CategoryBase, Guid, CategoryContext>(dbContext, unitOfWork), ICategoryRepository
{
    public async Task<IEnumerable<CategoryBase>> GetCategories() => await FindAll().ToListAsync();
}