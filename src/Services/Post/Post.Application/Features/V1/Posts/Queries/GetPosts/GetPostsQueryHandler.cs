using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Models;
using Post.Domain.Interfaces;
using Serilog;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPosts;

public class GetPostsQueryHandler(IPostRepository postRepository, IMapper mapper, ILogger logger)
    : IRequestHandler<GetPostsQuery, ApiResult<IEnumerable<PostDto>>>
{
    public async Task<ApiResult<IEnumerable<PostDto>>> Handle(GetPostsQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<IEnumerable<PostDto>>();

        try
        {
            var posts = await postRepository.GetPosts();
            if (posts.IsNotNullOrEmpty())
            {
                var data = mapper.Map<IEnumerable<PostDto>>(posts);
                result.Success(data);
            }
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetPostsQuery), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}