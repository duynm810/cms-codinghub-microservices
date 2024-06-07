using AutoMapper;
using Contracts.Commons.Interfaces;
using PostInTag.Api.Repositories.Interfaces;
using PostInTag.Api.Services.Interfaces;
using Shared.Dtos.PostInTag;
using Shared.Responses;
using ILogger = Serilog.ILogger;

namespace PostInTag.Api.Services;

public class PostInTagService(
    IPostInTagRepository postInTagRepository,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : IPostInTagService
{
    public async Task<ApiResult<bool>> CreatePostToTag(CreatePostInTagDto request)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResult<bool>> DeletePostToTag(DeletePostInTagDto request)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResult<IEnumerable<PostInTagDto>>> GetPostsInTag(Guid tagId)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResult<IEnumerable<PostInTagDto>>> GetPostsInTagBySlug(string tagSlug)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResult<PagedResponse<PostInTagDto>>> GetPostsInTagPaging(Guid tagId, int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResult<PagedResponse<PostInTagDto>>> GetPostsInTagBySlugPaging(string tagSlug, int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }
}