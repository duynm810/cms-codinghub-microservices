using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.Entities;
using Post.Domain.Interfaces;
using Serilog;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Commands.CreatePost;

public class CreatePostCommandHandler(IPostRepository postRepository, IMapper mapper, ILogger logger)
    : IRequestHandler<CreatePostCommand, ApiResult<Guid>>
{
    public async Task<ApiResult<Guid>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Guid>();

        try
        {
            var post = mapper.Map<PostBase>(request);
            var id = postRepository.CreatePost(post);
            await postRepository.SaveChangesAsync();
            result.Success(id);
        }
        catch (Exception e)
        {
            logger.Error("Method: {MethodName}. Message: {ErrorMessage}", nameof(CreatePostCommand), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}