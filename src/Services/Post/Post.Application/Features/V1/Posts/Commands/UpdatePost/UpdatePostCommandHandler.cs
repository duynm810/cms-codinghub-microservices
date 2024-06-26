using AutoMapper;
using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.GrpcClients;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Post.Queries;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Commands.UpdatePost;

public class UpdatePostCommandHandler(
    IPostRepository postRepository,
    ICategoryGrpcClient categoryGrpcClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger)
    : IRequestHandler<UpdatePostCommand, ApiResult<PostDto>>
{
    public async Task<ApiResult<PostDto>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostDto>();
        const string methodName = nameof(UpdatePostCommand);

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
            var category = await categoryGrpcClient.GetCategoryById(request.CategoryId);
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

            var data = mapper.Map<PostDto>(updatePost);
            result.Success(data);

            // Xóa cache liên quan
            var cacheKeys = new List<string>
            {
                CacheKeyHelper.Post.GetAllPostsKey(),
                CacheKeyHelper.Post.GetPostByIdKey(request.Id),
                CacheKeyHelper.Post.GetPinnedPostsKey(),
                CacheKeyHelper.Post.GetFeaturedPostsKey(),
                CacheKeyHelper.Post.GetPostBySlugKey(request.Slug),
                CacheKeyHelper.Post.GetLatestPostsPagingKey(1, 5),
                CacheKeyHelper.Post.GetPostsByCategoryPagingKey(category.Slug, 1, 6),
                CacheKeyHelper.Post.GetPostsByCurrentUserPagingKey(request.AuthorUserId, 1, 4)
            };

            await cacheService.RemoveMultipleAsync(cacheKeys, cancellationToken);

            logger.Information("END {MethodName} - Post with ID {PostId} updated successfully", methodName, request.Id);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}