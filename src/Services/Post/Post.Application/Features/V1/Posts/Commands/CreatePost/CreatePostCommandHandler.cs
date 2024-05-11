using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.Entities;
using Post.Domain.Repositories;
using Post.Domain.Services;
using Serilog;
using Shared.Constants;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Commands.CreatePost;

public class CreatePostCommandHandler(IPostRepository postRepository, ICategoryGrpcService categoryGrpcService, IMapper mapper, ILogger logger)
    : IRequestHandler<CreatePostCommand, ApiResult<Guid>>
{
    public async Task<ApiResult<Guid>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Guid>();

        try
        {
            // Check slug exists
            var slugExists = await postRepository.SlugExists(request.Slug);
            if (slugExists)
            {
                result.Messages.Add(ErrorMessageConsts.Post.SlugExists);
                result.Failure(StatusCodes.Status409Conflict, result.Messages);
                return result;
            }
            
            // Check valid category id
            var category = await categoryGrpcService.GetCategoryById(request.CategoryId);
            if (category == null)
            {
                result.Messages.Add(ErrorMessageConsts.Category.InvalidCategoryId);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }
            
            var post = mapper.Map<PostBase>(request);
            
            // Set category id get by categories services
            post.CategoryId = category.Id;
            
            var id = postRepository.CreatePost(post);
            await postRepository.SaveChangesAsync();
            result.Success(id);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(CreatePostCommand), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}