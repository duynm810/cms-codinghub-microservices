using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Infrastructure.Paged;
using Microsoft.EntityFrameworkCore;
using Series.Api.Entities;
using Series.Api.Persistence;
using Series.Api.Repositories.Interfaces;
using Shared.Responses;

namespace Series.Api.Repositories;

public class SeriesRepository(SeriesContext dbContext, IUnitOfWork<SeriesContext> unitOfWork)
    : RepositoryCommandBase<SeriesBase, Guid, SeriesContext>(dbContext, unitOfWork), ISeriesRepository
{
    #region CRUD

    public async Task CreateSeries(SeriesBase series) => await CreateAsync(series);

    public async Task<SeriesBase> UpdateSeries(SeriesBase series)
    {
        await UpdateAsync(series);
        return series;
    }

    public async Task DeleteSeries(SeriesBase series) => await DeleteAsync(series);

    public async Task<IEnumerable<SeriesBase>> GetSeries(int count = int.MaxValue) => await FindAll().Take(count).ToListAsync();

    public async Task<SeriesBase?> GetSeriesById(Guid id) => await GetByIdAsync(id) ?? null;

    #endregion

    #region OTHERS

    public async Task<PagedResponse<SeriesBase>> GetSeriesPaging(int pageNumber, int pageSize)
    {
        var query = FindAll();
        
        var items = await PagedList<SeriesBase>.ToPagedList(query, pageNumber, pageSize, x => x.CreatedDate);

        var response = new PagedResponse<SeriesBase>
        {
            Items = items,
            MetaData = items.GetMetaData()
        };

        return response;
    }

    #endregion
}