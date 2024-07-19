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

    Task<PostInSeriesBase?> GetPostInSeries(Guid postId, Guid seriesId);

    #endregion

    #region OTHERS

    Task<IEnumerable<Guid>?> GetPostIdsBySeriesId(Guid seriesId);

    Task<IEnumerable<Guid>> GetSeriesIdsByPostId(Guid postId);

    #endregion
}