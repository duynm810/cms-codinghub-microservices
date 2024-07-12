using Contracts.Domains.Repositories;
using Series.Api.Entities;
using Shared.Requests.Series;
using Shared.Responses;

namespace Series.Api.Repositories.Interfaces;

public interface ISeriesRepository : IRepositoryCommandBase<SeriesBase, Guid>
{
    #region CRUD

    Task CreateSeries(SeriesBase series);

    Task<SeriesBase> UpdateSeries(SeriesBase series);

    Task DeleteSeries(SeriesBase series);

    Task<IEnumerable<SeriesBase>> GetSeries();

    Task<SeriesBase?> GetSeriesById(Guid id);

    #endregion

    #region OTHERS

    Task<PagedResponse<SeriesBase>> GetSeriesPaging(GetSeriesRequest request);

    Task<SeriesBase?> GetSeriesBySlug(string slug);

    #endregion
}