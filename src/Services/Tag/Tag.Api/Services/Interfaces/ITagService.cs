using Shared.Dtos.Tag;
using Shared.Responses;

namespace Tag.Api.Services.Interfaces;

public interface ITagService
{
    #region CRUD

    Task<ApiResult<TagDto>> CreateTag(CreateTagDto request);

    Task<ApiResult<TagDto>> UpdateTag(Guid id, UpdateTagDto request);

    Task<ApiResult<bool>> DeleteTag(List<Guid> ids);

    Task<ApiResult<IEnumerable<TagDto>>> GetTags();

    Task<ApiResult<TagDto>> GetTagById(Guid id);

    #endregion
}