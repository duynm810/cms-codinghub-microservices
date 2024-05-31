using Contracts.Domains.Repositories;
using Series.Grpc.Entities;

namespace Series.Grpc.Repositories.Interfaces;

public interface ISeriesRepository : IRepositoryCommandBase<SeriesBase, Guid>
{
    #region CRUD

    Task<SeriesBase?> GetSeriesById(Guid id);
    
    #endregion

    #region OTHERS

    Task<SeriesBase?> GetSeriesBySlug(string slug);

    #endregion
}