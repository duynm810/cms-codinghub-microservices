using Shared.Dtos.Tag;
using Shared.Responses;
using WebApps.UI.ApiServices.Interfaces;

namespace WebApps.UI.ApiServices;

public class TagApiClient(IBaseApiClient baseApiClient) : ITagApiClient
{
    public async Task<ApiResult<List<TagDto>>> GetTags(int count)
    {
        return await baseApiClient.GetListAsync<TagDto>($"/tags?count={count}");
    }
    
    public async Task<ApiResult<TagDto>> GetTagBySlug(string slug)
    {
        return await baseApiClient.GetAsync<TagDto>($"/tags/by-slug/{slug}");
    }
}