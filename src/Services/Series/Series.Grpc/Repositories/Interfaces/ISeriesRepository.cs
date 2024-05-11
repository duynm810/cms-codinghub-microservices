using Contracts.Domains.Repositories;
using Series.Grpc.Entities;
using Shared.Responses;

namespace Series.Grpc.Repositories.Interfaces;

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

    Task<PagedResponse<SeriesBase>> GetSeriesPaging(int pageNumber, int pageSize);

    #endregion
}