using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Models;
using Post.Domain.Interfaces;
using Serilog;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Commands.UpdatePost;

public class UpdatePostCommandHandler(IPostRepository postRepository, ICategoryGrpcService categoryGrpcService, IMapper mapper, ILogger logger)
    : IRequestHandler<UpdatePostCommand, ApiResult<PostDto>>
{
    public async Task<ApiResult<PostDto>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostDto>();

        try
        {
            var post = await postRepository.GetPostById(request.Id);
            if (post == null)
            {
                result.Messages.Add("Post not found");
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }
            
            // Check slug exists
            var slugExists = await postRepository.SlugExists(request.Slug, request.Id);
            if (slugExists)
            {
                result.Messages.Add("Slug is exists");
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }

            // Check valid category id
            var category = await categoryGrpcService.GetCategoryById(request.CategoryId);
            if (category == null)
            {
                result.Messages.Add("Category not found");
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var updatePost = mapper.Map(request, post);
            await postRepository.UpdatePost(updatePost);

            var data = mapper.Map<PostDto>(updatePost);
            result.Success(data);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(UpdatePostCommand), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}