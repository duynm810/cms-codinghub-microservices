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
    public async Task<ApiResult<bool>> Handle(UpdateThumbnailCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(UpdateThumbnailCommand);

        try
        {
            logger.Information("BEGIN {MethodName} - Updating thumbnail for post with ID: {PostId}", methodName,
                request.Id);

            // Fetch the post
            var post = await postRepository.GetPostById(request.Id);
            if (post == null)
            {
                logger.Warning("{MethodName} - Post not found with ID: {PostId}", methodName, request.Id);
                result.Messages.Add("Post not found.");
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            // Update the thumbnail
            post.Thumbnail = request.Thumbnail;

            await postRepository.UpdatePost(post);

            result.Success(true);

            // Xóa cache liên quan
            var cacheKeys = new List<string>
            {
                CacheKeyHelper.Post.GetAllPostsKey(),
                CacheKeyHelper.Post.GetPostByIdKey(request.Id),
                CacheKeyHelper.Post.GetPinnedPostsKey(),
                CacheKeyHelper.Post.GetFeaturedPostsKey(),
                CacheKeyHelper.Post.GetPostBySlugKey(post.Slug),
                CacheKeyHelper.Post.GetLatestPostsPagingKey(1, 10),
                CacheKeyHelper.Post.GetPostsByCurrentUserPagingKey(post.AuthorUserId, 1, 4)
            };

            await cacheService.RemoveMultipleAsync(cacheKeys, cancellationToken);
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