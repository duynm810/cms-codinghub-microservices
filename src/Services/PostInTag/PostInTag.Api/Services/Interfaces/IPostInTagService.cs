using Shared.Dtos.PostInTag;
using Shared.Responses;

namespace PostInTag.Api.Services.Interfaces;

public interface IPostInTagService
{
    Task<ApiResult<bool>> CreatePostToTag(CreatePostInTagDto request);

    Task<ApiResult<bool>> DeletePostToTag(DeletePostInTagDto request);
}