using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Post.Domain.Entities;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Commands.CreatePost;

public class CreatePostCommandHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    IDistributedCache redisCacheService,
    IMapper mapper,
    ILogger logger)
    : IRequestHandler<CreatePostCommand, ApiResult<Guid>>
{
    public async Task<ApiResult<Guid>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Guid>();
        const string methodName = nameof(Handle);

        try
        {
            logger.Information("BEGIN {MethodName} - Creating post with name: {Name}", methodName, request.Name);

            // Check slug exists
            var slugExists = await postRepository.SlugExists(request.Slug);
            if (slugExists)
            {
                logger.Warning("{MethodName} - Slug already exists: {PostSlug}", methodName, request.Slug);
                result.Messages.Add(ErrorMessagesConsts.Post.SlugExists);
                result.Failure(StatusCodes.Status409Conflict, result.Messages);
                return result;
            }

            // Check valid category id
            var category = await categoryGrpcService.GetCategoryById(request.CategoryId);
            if (category == null)
            {
                logger.Warning("{MethodName} - Invalid category ID: {CategoryId}", methodName, request.CategoryId);
                result.Messages.Add(ErrorMessagesConsts.Category.InvalidCategoryId);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }

            var post = mapper.Map<PostBase>(request);
            
            // Set category id get by categories services
            post.CategoryId = category.Id;

            var id = postRepository.CreatePost(post);
            await postRepository.SaveChangesAsync();
            result.Success(id);
            
            // Xóa cache liên quan
            await redisCacheService.RemoveAsync("all_posts", cancellationToken);
            await redisCacheService.RemoveAsync($"posts_by_category_{category.Slug}_page_1_size_10", cancellationToken);

            logger.Information("END {MethodName} - Post created successfully with ID: {PostId}", methodName, id);
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