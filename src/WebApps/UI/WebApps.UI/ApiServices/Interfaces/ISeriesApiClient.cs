using Shared.Dtos.Series;
using Shared.Responses;

namespace WebApps.UI.ApiServices.Interfaces;

public interface ISeriesApiClient
{
    Task<ApiResult<List<SeriesDto>>> GetSeries(int count);

    Task<ApiResult<SeriesDto>> GetSeriesBySlug(string slug);
}