using Shared.Dtos.Series;
using Shared.Responses;

namespace WebApps.UI.Services.Interfaces;

public interface ISeriesApiClient
{
    Task<ApiResult<List<SeriesDto>>> GetSeries();

    Task<ApiResult<SeriesDto>> GetSeriesBySlug(string slug);
}