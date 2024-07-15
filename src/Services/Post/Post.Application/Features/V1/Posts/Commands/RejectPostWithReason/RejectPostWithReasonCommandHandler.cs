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

namespace Post.Application.Features.V1.Posts.Commands.RejectPostWithReason;

public class RejectPostWithReasonCommandHandler(
    IPostRepository postRepository,
    IPostActivityLogRepository postActivityLogRepository,
    IPostEmailTemplateService postEmailTemplateService,
    ILogger logger) : IRequestHandler<RejectPostWithReasonCommand, ApiResult<bool>>
{
    public async Task<ApiResult<bool>> Handle(RejectPostWithReasonCommand command, CancellationToken cancellationToken)
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

                await postRepository.RejectPostWithReason(post);

                var postActivityLog = new PostActivityLog
                {
                    Id = Guid.NewGuid(),
                    FromStatus = post.Status,
                    ToStatus = PostStatusEnum.Rejected,
                    UserId = command.UserId,
                    PostId = command.Id,
                    Note = command.Request.Reason
                };
                await postActivityLogRepository.CreatePostActivityLogs(postActivityLog);

                await postRepository.SaveChangesAsync();
                await postRepository.EndTransactionAsync();

                result.Success(true);

                try
                {
                    TaskHelper.RunFireAndForget(async () =>
                    {
                        // Send email to author
                        await postEmailTemplateService.SendPostRejectionEmail(post.Title, postActivityLog.Note)
                            .ConfigureAwait(false);
                    }, e =>
                    {
                        logger.Error("Send rejection post email failed. Message: {ErrorMessage}", e.Message);
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