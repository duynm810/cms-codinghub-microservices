using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Infrastructure.Paged;
using Microsoft.EntityFrameworkCore;
using Series.Grpc.Entities;
using Series.Grpc.Persistence;
using Series.Grpc.Repositories.Interfaces;
using Shared.Responses;

namespace Series.Grpc.Repositories;

public class SeriesRepository(SeriesContext dbContext, IUnitOfWork<SeriesContext> unitOfWork)
    : RepositoryCommandBase<SeriesBase, Guid, SeriesContext>(dbContext, unitOfWork), ISeriesRepository
{
    #region CRUD

    public async Task<SeriesBase?> GetSeriesById(Guid id) => await GetByIdAsync(id) ?? null;

    public async Task<IEnumerable<SeriesBase>> GetAllSeries() => await FindAll().ToListAsync();

    #endregion

    #region OTHERS

    public async Task<SeriesBase?> GetSeriesBySlug(string slug) =>
        await FindByCondition(x => x.Slug == slug).FirstOrDefaultAsync();

    public async Task<IEnumerable<SeriesBase>> GetSeriesByIds(Guid[] ids) =>
        await FindByCondition(c => ids.Contains(c.Id)).ToListAsync();

    #endregion
}