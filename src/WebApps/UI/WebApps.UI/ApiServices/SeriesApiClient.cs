using Shared.Dtos.Series;
using Shared.Responses;
using WebApps.UI.ApiServices.Interfaces;

namespace WebApps.UI.ApiServices;

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