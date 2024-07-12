using Shared.Dtos.Tag;
using Shared.Responses;

namespace WebApps.UI.ApiClients.Interfaces;

public interface ITagApiClient
{
    Task<ApiResult<List<TagDto>>> GetTags(int count);

    Task<ApiResult<TagDto>> GetTagBySlug(string slug);
    
    Task<ApiResult<TagDto>> GetTagByName(string name);

    Task<ApiResult<List<TagDto>>> GetSuggestedTags(int count);
}