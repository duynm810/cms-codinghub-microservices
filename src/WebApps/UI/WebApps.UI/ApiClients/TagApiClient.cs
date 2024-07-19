using Shared.Dtos.Tag;
using Shared.Responses;
using WebApps.UI.ApiClients.Interfaces;

namespace WebApps.UI.ApiClients;

public class TagApiClient(IBaseApiClient baseApiClient) : ITagApiClient
{
    public async Task<ApiResult<List<TagDto>>> GetTags(int count)
    {
        return await baseApiClient.GetListAsync<TagDto>($"/tags?count={count}");
    }
    
    public async Task<ApiResult<List<TagDto>>> GetSuggestedTags(int count)
    {
        return await baseApiClient.GetListAsync<TagDto>($"/tags/suggest?count={count}");
    }
}