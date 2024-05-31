using Shared.Dtos.Series;
using Shared.Responses;
using WebApps.UI.Services.Interfaces;

namespace WebApps.UI.Services;

public class SeriesApiClient(IBaseApiClient baseApiClient)  : ISeriesApiClient
{
    public async Task<ApiResult<List<SeriesDto>>> GetSeries()
    {
        return await baseApiClient.GetListAsync<SeriesDto>("/series");
    }
    
    public async Task<ApiResult<SeriesDto>> GetSeriesBySlug(string slug)
    {
        return await baseApiClient.GetAsync<SeriesDto>($"/series/by-slug/{slug}");
    }
}