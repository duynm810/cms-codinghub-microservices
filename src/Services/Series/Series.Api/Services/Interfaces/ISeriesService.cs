using Shared.Dtos.Series;
using Shared.Responses;

namespace Series.Api.Services.Interfaces;

public interface ISeriesService
{
    #region CRUD

    Task<ApiResult<SeriesDto>> CreateSeries(CreateSeriesDto request);

    Task<ApiResult<SeriesDto>> UpdateSeries(Guid id, UpdateSeriesDto request);

    Task<ApiResult<bool>> DeleteSeries(List<Guid> ids);

    Task<ApiResult<IEnumerable<SeriesDto>>> GetSeries(int count);

    Task<ApiResult<SeriesDto>> GetSeriesById(Guid id);

    #endregion

    #region OTHERS

    Task<ApiResult<PagedResponse<SeriesDto>>> GetSeriesPaging(int pageNumber, int pageSize);

    Task<ApiResult<SeriesDto>> GetSeriesBySlug(string slug);

    #endregion
}