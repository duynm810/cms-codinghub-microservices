using Shared.Dtos.Series;
using Shared.Requests.Series;
using Shared.Responses;

namespace Series.Api.Services.Interfaces;

public interface ISeriesService
{
    #region CRUD

    Task<ApiResult<SeriesDto>> CreateSeries(CreateSeriesRequest request);

    Task<ApiResult<SeriesDto>> UpdateSeries(Guid id, UpdateSeriesRequest request);

    Task<ApiResult<bool>> DeleteSeries(List<Guid> ids);

    Task<ApiResult<IEnumerable<SeriesDto>>> GetSeries();

    Task<ApiResult<SeriesDto>> GetSeriesById(Guid id);

    #endregion

    #region OTHERS

    Task<ApiResult<PagedResponse<SeriesDto>>> GetSeriesPaging(GetSeriesRequest request);

    Task<ApiResult<SeriesDto>> GetSeriesBySlug(string slug);

    #endregion
}