using AutoMapper;
using Contracts.Commons.Interfaces;
using PostInTag.Api.Entities;
using PostInTag.Api.Repositories.Interfaces;
using PostInTag.Api.Services.Interfaces;
using Shared.Dtos.PostInTag;
using Shared.Helpers;
using Shared.Requests.PostInTag;
using Shared.Responses;
using Shared.Utilities;
using ILogger = Serilog.ILogger;

namespace PostInTag.Api.Services;

public class PostInTagService(
    IPostInTagRepository postInTagRepository,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : IPostInTagService
{
    public async Task<ApiResult<bool>> CreatePostToTag(CreatePostInTagRequest request)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(CreatePostToTag);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Creating post with ID: {PostId} to tag with ID: {TagId} with sort order: {SortOrder}",
                methodName, request.PostId, request.TagId, request.SortOrder);

            var postInTag = mapper.Map<PostInTagBase>(request);

            await postInTagRepository.CreatePostToTag(postInTag);
            result.Success(true);

            TaskHelper.RunFireAndForget(async () =>
            {
                var cacheKeys = new List<string>
                {
                    CacheKeyHelper.PostInTag.GetAllPostInTagByIdKey(request.TagId)
                };

                await cacheService.RemoveMultipleAsync(cacheKeys);
            }, e =>
            {
                logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            });
            
            logger.Information(
                "END {MethodName} - Successfully created post with ID: {PostId} to tag with ID: {TagId}",
                methodName, request.PostId, request.TagId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> DeletePostToTag(DeletePostInTagRequest request)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(DeletePostToTag);

        try
        {
            logger.Information("BEGIN {MethodName} - Deleting post with ID: {PostId} from tag with ID: {TagId}",
                methodName, request.PostId, request.TagId);

            var postInTag = mapper.Map<PostInTagBase>(request);

            await postInTagRepository.DeletePostToTag(postInTag);
            result.Success(true);
            
            TaskHelper.RunFireAndForget(async () =>
            {
                var cacheKeys = new List<string>
                {
                    CacheKeyHelper.PostInTag.GetAllPostInTagByIdKey(request.TagId)
                };

                await cacheService.RemoveMultipleAsync(cacheKeys);
            }, e =>
            {
                logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            });

            logger.Information(
                "END {MethodName} - Successfully deleted post with ID: {PostId} from tag with ID: {TagId}",
                methodName, request.PostId, request.TagId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}