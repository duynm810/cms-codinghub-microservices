using EventBus.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Post.Domain.Entities;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Enums;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Commands.ApprovePost;

public class ApprovePostCommandHandler(
    IPostRepository postRepository,
    IPostActivityLogRepository postActivityLogRepository,
    IPublishEndpoint publishEndpoint,
    ILogger logger) : IRequestHandler<ApprovePostCommand, ApiResult<bool>>
{
    public async Task<ApiResult<bool>> Handle(ApprovePostCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(Handle);

        var strategy = postRepository.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await postRepository.BeginTransactionAsync();
            try
            {
                var post = await postRepository.GetPostById(request.Id);
                if (post == null)
                {
                    logger.Warning("{MethodName} - Post not found with ID: {PostId}", methodName, request.Id);
                    result.Messages.Add(ErrorMessageConsts.Post.PostNotFound);
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                    return result;
                }

                // TODO: Implement check current user id

                await postRepository.ApprovePost(post);

                var postActivityLog = new PostActivityLog
                {
                    Id = Guid.NewGuid(),
                    FromStatus = post.Status,
                    ToStatus = PostStatusEnum.Published,
                    UserId = Guid.NewGuid(), // TODO: Replace with current user ID
                    PostId = request.Id
                };
                await postActivityLogRepository.CreatePostActivityLogs(postActivityLog);

                await postRepository.SaveChangesAsync();
                await postRepository.EndTransactionAsync();
                
                // Publish PostApprovedEvent event
                var postApprovedEvent = new PostApprovedEvent
                {
                    PostId = post.Id,
                    Name = post.Name,
                    Content = post.Content,
                    Description = post.Description
                };

                await publishEndpoint.Publish(postApprovedEvent, cancellationToken);

                result.Success(true);
            }
            catch (Exception e)
            {
                await postRepository.RollbackTransactionAsync();
                logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(ApprovePostCommand), e);
                result.Messages.AddRange(e.GetExceptionList());
                result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
            }

            return result;
        });

        return result;
    }
    
    
}