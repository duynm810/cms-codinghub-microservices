using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.Repositories;
using Serilog;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Commands.UpdateThumbnail;

public class UpdateThumbnailCommandHandler(
    IPostRepository postRepository,
    ICacheService cacheService,
    ILogger logger) : IRequestHandler<UpdateThumbnailCommand, ApiResult<bool>>
{
    public async Task<ApiResult<bool>> Handle(UpdateThumbnailCommand command, CancellationToken cancellationToken)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(UpdateThumbnailCommand);

        try
        {
            logger.Information("BEGIN {MethodName} - Updating thumbnail for post with ID: {PostId}", methodName,
                command.Id);

            // Fetch the post
            var post = await postRepository.GetPostById(command.Id);
            if (post == null)
            {
                logger.Warning("{MethodName} - Post not found with ID: {PostId}", methodName, command.Id);
                result.Messages.Add("Post not found.");
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            // Update the thumbnail
            post.ThumbnailFileId = command.ThumbnailFileId;

            await postRepository.UpdatePost(post);

            result.Success(true);

            TaskHelper.RunFireAndForget(async () =>
            {
                var cacheKeys = new List<string>
                {
                    CacheKeyHelper.Post.GetAllPostsKey(),
                    CacheKeyHelper.Post.GetPinnedPostsKey(),
                    CacheKeyHelper.Post.GetFeaturedPostsKey(),
                    CacheKeyHelper.Post.GetMostLikedPostsKey(),
                    CacheKeyHelper.Post.GetMostCommentPostsKey(),
                    CacheKeyHelper.Post.GetPostByIdKey(post.Id),
                    CacheKeyHelper.Post.GetPostBySlugKey(post.Slug),
                    CacheKeyHelper.Post.GetDetailBySlugKey(post.Slug),
                    CacheKeyHelper.Post.GetPostsByNonStaticPageCategoryKey()
                };

                await cacheService.RemoveMultipleAsync(cacheKeys, cancellationToken);
            }, e =>
            {
                logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            });
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