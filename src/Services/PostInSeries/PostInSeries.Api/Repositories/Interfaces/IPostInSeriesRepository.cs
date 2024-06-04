using Contracts.Domains.Repositories;
using PostInSeries.Api.Entities;

namespace PostInSeries.Api.Repositories.Interfaces;

public interface IPostInSeriesRepository : IRepositoryCommandBase<PostInSeriesBase, Guid>
{
    #region CRUD

    Task CreatePostToSeries(PostInSeriesBase postInSeriesBase);

    Task DeletePostToSeries(PostInSeriesBase postInSeriesBase);

    #endregion

    #region OTHERS

    Task<IEnumerable<Guid>?> GetPostIdsInSeries(Guid seriesId);

    #endregion
}