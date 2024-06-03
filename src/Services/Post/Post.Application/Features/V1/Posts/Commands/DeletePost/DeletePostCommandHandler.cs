using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Commands.DeletePost;

public class DeletePostCommandHandler(IPostRepository postRepository, IDistributedCache redisCacheService, ILogger logger)
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
            await redisCacheService.RemoveAsync("all_posts", cancellationToken);
            await redisCacheService.RemoveAsync($"post_{request.Id}", cancellationToken);
            await redisCacheService.RemoveAsync($"posts_by_category_{post.CategoryId}_page_1_size_10", cancellationToken);

            logger.Information("END {MethodName} - Post with ID {PostId} deleted successfully", methodName, request.Id);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(DeletePostCommand), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}