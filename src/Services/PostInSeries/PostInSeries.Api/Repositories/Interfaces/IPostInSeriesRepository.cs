
namespace PostInSeries.Api.Repositories.Interfaces;

public interface IPostInSeriesRepository
{
    #region CRUD

    Task CreatePostToSeries(Guid seriesId, Guid postId, int sortOrder);

    Task DeletePostToSeries(Guid seriesId, Guid postId);

    #endregion

    #region OTHERS

    Task<IEnumerable<Guid>?> GetPostIdsInSeries(Guid seriesId);

    #endregion
}