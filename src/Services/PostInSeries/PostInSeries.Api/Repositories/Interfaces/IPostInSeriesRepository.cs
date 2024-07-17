using Contracts.Domains.Repositories;
using PostInSeries.Api.Entities;

namespace PostInSeries.Api.Repositories.Interfaces;

public interface IPostInSeriesRepository : IRepositoryCommandBase<PostInSeriesBase, Guid>
{
    #region CRUD

    Task CreatePostToSeries(PostInSeriesBase postInSeriesBase);
    
    Task CreatePostsToSeries(IEnumerable<PostInSeriesBase> postInSeriesList);

    Task DeletePostToSeries(PostInSeriesBase postInSeriesBase);
    
    Task DeletePostToSeries(Guid postId);

    #endregion

    #region OTHERS

    Task<IEnumerable<Guid>?> GetPostIdsInSeries(Guid seriesId);

    #endregion
}