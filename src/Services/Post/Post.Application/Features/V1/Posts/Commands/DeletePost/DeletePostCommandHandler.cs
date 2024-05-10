using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.Repositories;
using Serilog;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Commands.DeletePost;

public class DeletePostCommandHandler(IPostRepository postRepository, ILogger logger)
    : IRequestHandler<DeletePostCommand, ApiResult<bool>>
{
    public async Task<ApiResult<bool>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<bool>();

        try
        {
            var post = await postRepository.GetPostById(request.Id);
            if (post == null)
            {
                result.Messages.Add("Post not found");
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            await postRepository.DeletePost(post);
            result.Success(true);
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