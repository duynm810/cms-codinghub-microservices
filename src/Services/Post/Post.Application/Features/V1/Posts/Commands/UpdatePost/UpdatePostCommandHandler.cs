using AutoMapper;
using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Post;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Commands.UpdatePost;

public class UpdatePostCommandHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger)
    : IRequestHandler<UpdatePostCommand, ApiResult<PostModel>>
{
    public async Task<ApiResult<PostModel>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostModel>();
        const string methodName = nameof(Handle);

        try
        {
            logger.Information("BEGIN {MethodName} - Updating post with ID: {PostId}", methodName, request.Id);

            var post = await postRepository.GetPostById(request.Id);
            if (post == null)
            {
                logger.Warning("{MethodName} - Post not found with ID: {PostId}", methodName, request.Id);
                result.Messages.Add(ErrorMessagesConsts.Post.PostNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            // Check slug exists
            var slugExists = await postRepository.SlugExists(request.Slug, request.Id);
            if (slugExists)
            {
                logger.Warning("{MethodName} - Slug already exists for post with ID: {PostId}, Slug: {PostSlug}",
                    methodName, request.Id, request.Slug);
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

            var updatePost = mapper.Map(request, post);

            // Set category id get by categories services
            updatePost.CategoryId = category.Id;

            await postRepository.UpdatePost(updatePost);

            var data = mapper.Map<PostModel>(updatePost);
            result.Success(data);

            // Xóa cache liên quan
            var cacheKeys = new List<string>
            {
                CacheKeyHelper.Post.GetAllPostsKey(),
                CacheKeyHelper.Post.GetPostByIdKey(request.Id),
                CacheKeyHelper.Post.GetPinnedPostsKey(),
                CacheKeyHelper.Post.GetFeaturedPostsKey()
            };

            await cacheService.RemoveMultipleAsync(cacheKeys, cancellationToken);

            logger.Information("END {MethodName} - Post with ID {PostId} updated successfully", methodName, request.Id);
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