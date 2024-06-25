using AutoMapper;
using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.Entities;
using Post.Domain.GrpcClients;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Commands.CreatePost;

public class CreatePostCommandHandler(
    IPostRepository postRepository,
    ICategoryGrpcClient categoryGrpcClient,
    ICacheService cacheService,
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
            logger.Information("BEGIN {MethodName} - Creating post with title: {Title}", methodName, request.Title);

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
            var category = await categoryGrpcClient.GetCategoryById(request.CategoryId);
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
            var cacheKeys = new List<string>
            {
                CacheKeyHelper.Post.GetAllPostsKey(),
                CacheKeyHelper.Post.GetPostByIdKey(id),
                CacheKeyHelper.Post.GetPinnedPostsKey(),
                CacheKeyHelper.Post.GetFeaturedPostsKey(),
                CacheKeyHelper.Post.GetPostBySlugKey(request.Slug),
                CacheKeyHelper.Post.GetLatestPostsPagingKey(1, 10),
                CacheKeyHelper.Post.GetPostsByCategoryPagingKey(category.Slug, 1, 10),
                CacheKeyHelper.Post.GetPostsByCurrentUserPagingKey(request.AuthorUserId, 1, 4)
            };

            await cacheService.RemoveMultipleAsync(cacheKeys, cancellationToken);

            logger.Information("END {MethodName} - Post created successfully with ID: {PostId}", methodName, id);
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