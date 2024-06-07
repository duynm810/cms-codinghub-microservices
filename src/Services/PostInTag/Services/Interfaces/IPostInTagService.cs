using Shared.Dtos.PostInTag;
using Shared.Responses;

namespace PostInTag.Api.Services.Interfaces;

public interface IPostInTagService
{
    Task<ApiResult<bool>> CreatePostToTag(CreatePostInTagDto request);

    Task<ApiResult<bool>> DeletePostToTag(DeletePostInTagDto request);

    Task<ApiResult<IEnumerable<PostInTagDto>>> GetPostsInTag(Guid tagId);

    Task<ApiResult<IEnumerable<PostInTagDto>>> GetPostsInTagBySlug(string tagSlug);

    Task<ApiResult<PagedResponse<PostInTagDto>>> GetPostsInTagPaging(Guid tagId, int pageNumber, int pageSize);

    Task<ApiResult<PagedResponse<PostInTagDto>>> GetPostsInTagBySlugPaging(string tagSlug, int pageNumber, int pageSize);
}