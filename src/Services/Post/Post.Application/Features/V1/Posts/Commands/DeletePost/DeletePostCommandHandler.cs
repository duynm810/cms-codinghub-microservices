using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.Repositories;
using Post.Domain.Services;
using Serilog;
using Shared.Constants;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Commands.DeletePost;

public class DeletePostCommandHandler(
    IPostRepository postRepository,
    ICacheService cacheService,
    IPostEventService postEventService,
    ILogger logger)
    : IRequestHandler<DeletePostCommand, ApiResult<bool>>
{
    public async Task<ApiResult<bool>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(Handle);

        try
        {
            logger.Information("BEGIN {MethodName} - Deleting post with ID: {PostId}", methodName, request.Id);

            var post = await postRepository.GetPostById(request.Id);
            if (post == null)
            {
                logger.Warning("{MethodName} - Post not found with ID: {PostId}", methodName, request.Id);
                result.Messages.Add(ErrorMessagesConsts.Post.PostNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            await postRepository.DeletePost(post);
            result.Success(true);

            // Xóa cache liên quan
            TaskHelper.RunFireAndForget(async () =>
            {
                var cacheKeys = new List<string>
                {
                    CacheKeyHelper.Post.GetAllPostsKey(),
                    CacheKeyHelper.Post.GetPostByIdKey(request.Id),
                    CacheKeyHelper.Post.GetPinnedPostsKey(),
                    CacheKeyHelper.Post.GetFeaturedPostsKey(),
                    CacheKeyHelper.Post.GetPostBySlugKey(post.Slug)
                };

                await cacheService.RemoveMultipleAsync(cacheKeys, cancellationToken);
            }, e => { logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e); });

            TaskHelper.RunFireAndForget(() => postEventService.HandlePostDeletedEvent(request.Id), e =>
            {
                logger.Error("HandlePostDeletedEvent failed. Message: {ErrorMessage}", e.Message);
            });

            logger.Information("END {MethodName} - Post with ID {PostId} deleted successfully", methodName, request.Id);
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