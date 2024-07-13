using Category.Api.Entities;
using Category.Api.Persistence;
using Category.Api.Repositories.Interfaces;
using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Infrastructure.Paged;
using Microsoft.EntityFrameworkCore;
using Shared.Requests.Category;
using Shared.Responses;

namespace Category.Api.Repositories;

public class CategoryRepository(CategoryContext dbContext, IUnitOfWork<CategoryContext> unitOfWork)
    : RepositoryCommandBase<CategoryBase, long, CategoryContext>(dbContext, unitOfWork), ICategoryRepository
{
    #region CRUD

    public async Task CreateCategory(CategoryBase category) => await CreateAsync(category);

    public async Task UpdateCategory(CategoryBase category) => await UpdateAsync(category);

    public async Task DeleteCategory(CategoryBase category) => await DeleteAsync(category);

    public async Task<IEnumerable<CategoryBase>> GetCategories() => await FindAll().ToListAsync();

    public async Task<CategoryBase?> GetCategoryById(long id) => await GetByIdAsync(id) ?? null;

    #endregion

    #region OTHERS

    public async Task<PagedResponse<CategoryBase>> GetCategoriesPaging(GetCategoriesRequest request)
    {
        var query = FindAll();

        var items = await PagedList<CategoryBase>.ToPagedList(query, request.PageNumber, request.PageSize, x => x.SortOrder);

        var response = new PagedResponse<CategoryBase>
        {
            Items = items,
            MetaData = items.GetMetaData()
        };

        return response;
    }

    public async Task<CategoryBase?> GetCategoryBySlug(string slug) =>
        await FindByCondition(x => x.Slug == slug).FirstOrDefaultAsync() ?? null;

    #endregion
}