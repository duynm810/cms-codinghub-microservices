using Shared.Dtos.Tag;
using Shared.Responses;

namespace WebApps.UI.ApiServices.Interfaces;

public interface ITagApiClient
{
    Task<ApiResult<List<TagDto>>> GetTags(int count);
}