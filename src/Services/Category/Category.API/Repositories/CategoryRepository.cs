using Category.API.Entities;
using Category.API.Persistence;
using Category.API.Repositories.Interfaces;
using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Infrastructure.Paged;
using Microsoft.EntityFrameworkCore;
using Shared.Responses;

namespace Category.API.Repositories;

public class CategoryRepository(CategoryContext dbContext, IUnitOfWork<CategoryContext> unitOfWork)
    : RepositoryCommandBase<CategoryBase, Guid, CategoryContext>(dbContext, unitOfWork), ICategoryRepository
{
    #region CRUD

    public async Task CreateCategory(CategoryBase category) => await CreateAsync(category);

    public async Task UpdateCategory(CategoryBase category) => await UpdateAsync(category);

    public async Task DeleteCategory(CategoryBase category) => await DeleteAsync(category);

    public async Task<IEnumerable<CategoryBase>> GetCategories() => await FindAll().ToListAsync();

    public async Task<CategoryBase?> GetCategoryById(Guid id) => await GetByIdAsync(id) ?? null;

    #endregion

    #region OTHERS

    public async Task<PagedResponse<CategoryBase>> GetCategoriesPaging(int pageNumber, int pageSize)
    {
        var query = FindAll();

        var items = await PagedList<CategoryBase>.ToPagedList(query, pageNumber, pageSize, x => x.SortOrder);

        var response = new PagedResponse<CategoryBase>
        {
            Items = items,
            MetaData = items.GetMetaData()
        };

        return response;
    }

    #endregion
}