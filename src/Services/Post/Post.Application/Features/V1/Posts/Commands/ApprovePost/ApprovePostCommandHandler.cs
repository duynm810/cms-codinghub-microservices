using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Post.Domain.Entities;
using Post.Domain.Repositories;
using Post.Domain.Services;
using Serilog;
using Shared.Constants;
using Shared.Enums;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Commands.ApprovePost;

public class ApprovePostCommandHandler(
    IPostRepository postRepository,
    IPostActivityLogRepository postActivityLogRepository,
    IPostEmailTemplateService postEmailTemplateService,
    ICacheService cacheService,
    ILogger logger) : IRequestHandler<ApprovePostCommand, ApiResult<bool>>
{
    public async Task<ApiResult<bool>> Handle(ApprovePostCommand command, CancellationToken cancellationToken)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(Handle);

        var strategy = postRepository.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await postRepository.BeginTransactionAsync();
            try
            {
                var post = await postRepository.GetPostById(command.Id);
                if (post == null)
                {
                    logger.Warning("{MethodName} - Post not found with ID: {PostId}", methodName, command.Id);
                    result.Messages.Add(ErrorMessagesConsts.Post.PostNotFound);
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                    return result;
                }

                var oldStatus = post.Status;

                await postRepository.ApprovePost(post);

                var postActivityLog = new PostActivityLog
                {
                    Id = Guid.NewGuid(),
                    FromStatus = oldStatus,
                    ToStatus = PostStatusEnum.Published,
                    UserId = command.UserId,
                    PostId = command.Id
                };
                await postActivityLogRepository.CreatePostActivityLogs(postActivityLog);

                await postRepository.SaveChangesAsync();
                await postRepository.EndTransactionAsync();
                
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
                        CacheKeyHelper.Post.GetPostsByNonStaticPageCategoryKey()
                    };

                    await cacheService.RemoveMultipleAsync(cacheKeys, cancellationToken);
                }, e =>
                {
                    logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
                });

                try
                {
                    TaskHelper.RunFireAndForget(async () =>
                    {
                        // Send email to author
                        await postEmailTemplateService.SendApprovedPostEmail(post.Id, post.Title, post.Content, post.Summary).ConfigureAwait(false);
                    }, e =>
                    {
                        logger.Error("Send approved post email failed. Message: {ErrorMessage}", e.Message);
                    });
                }
                catch (Exception emailEx)
                {
                    logger.Error("{MethodName} - Error sending email for Post ID: {PostId}. Message: {ErrorMessage}",
                        methodName, command.Id, emailEx);
                    result.Messages.Add("Error sending email: " + emailEx.Message);
                    result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
                    throw;
                }
            }
            catch (Exception e)
            {
                await postRepository.RollbackTransactionAsync();
                logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
                result.Messages.AddRange(e.GetExceptionList());
                result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
            }

            return result;
        });

        return result;
    }
}