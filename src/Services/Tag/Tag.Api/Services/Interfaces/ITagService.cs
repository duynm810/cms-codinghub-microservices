using Shared.Dtos.Tag;
using Shared.Requests.Tag;
using Shared.Responses;

namespace Tag.Api.Services.Interfaces;

public interface ITagService
{
    #region CRUD

    Task<ApiResult<TagDto>> CreateTag(CreateTagRequest request);

    Task<ApiResult<TagDto>> UpdateTag(Guid id, UpdateTagRequest request);

    Task<ApiResult<bool>> DeleteTag(List<Guid> ids);

    Task<ApiResult<IEnumerable<TagDto>>> GetTags(int count);

    Task<ApiResult<TagDto>> GetTagById(Guid id);

    #endregion

    #region OTHERS

    Task<ApiResult<TagDto>> GetTagBySlug(string slug);
    
    Task<ApiResult<TagDto>> GetTagByName(string name);

    Task<ApiResult<IEnumerable<TagDto>>> GetSuggestedTags(string? keyword, int count);

    #endregion
}