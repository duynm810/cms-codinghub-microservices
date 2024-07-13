using Shared.Requests.PostInTag;
using Shared.Responses;

namespace PostInTag.Api.Services.Interfaces;

public interface IPostInTagService
{
    Task<ApiResult<bool>> CreatePostToTag(CreatePostInTagRequest request);

    Task<ApiResult<bool>> DeletePostToTag(DeletePostInTagRequest request);
}