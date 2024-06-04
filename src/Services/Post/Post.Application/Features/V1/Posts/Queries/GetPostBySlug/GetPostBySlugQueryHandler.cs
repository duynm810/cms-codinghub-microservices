using AutoMapper;
using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Helpers;
using Shared.Responses;
using Shared.Settings;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostBySlug;

public class GetPostBySlugQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    IDistributedCache redisCacheService,
    ISerializeService serializeService,
    ICacheService cacheService,
    DisplaySettings displaySettings,
    IMappingHelper mappingHelper,
    ILogger logger)
    : IRequestHandler<GetPostBySlugQuery, ApiResult<PostDetailModel>>
{
    public async Task<ApiResult<PostDetailModel>> Handle(GetPostBySlugQuery request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostDetailModel>();
        const string methodName = nameof(GetPostBySlugQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving post with slug: {PostSlug}", methodName, request.Slug);
            
            // Kiểm tra cache
            var cacheKey = CacheKeyHelper.Post.GetPostBySlugKey(request.Slug);
            var cachedPost = await cacheService.GetAsync<PostDetailModel>(cacheKey, cancellationToken);
            if (cachedPost != null)
            {
                result.Success(cachedPost);
                logger.Information("END {MethodName} - Successfully retrieved post from cache with slug: {PostSlug}", methodName, request.Slug);
                return result;
            }

            var post = await postRepository.GetPostBySlug(request.Slug);
            if (post == null)
            {
                logger.Warning("{MethodName} - Post not found with slug: {PostSlug}", methodName, request.Slug);
                result.Messages.Add(ErrorMessagesConsts.Post.PostNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }
            
            // Lấy danh mục và bài viết liên quan đồng thời
            var categoryTask = categoryGrpcService.GetCategoryById(post.CategoryId);
            var relatedPostsTask = postRepository.GetRelatedPosts(post,
                displaySettings.Config.GetValueOrDefault(DisplaySettingsConsts.Post.RelatedPosts, 0));

            await Task.WhenAll(categoryTask, relatedPostsTask);

            var category = categoryTask.Result;
            if (category == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }
            
            var data = new PostDetailModel()
            {
                DetailPost = mappingHelper.MapPostWithCategory(post, category)
            };
            
            var relatedPosts = relatedPostsTask.Result.ToList();
            if (relatedPosts.IsNotNullOrEmpty())
            {
                var categoryIds = relatedPosts.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcService.GetCategoriesByIds(categoryIds);

                data.RelatedPosts = mappingHelper.MapPostsWithCategories(relatedPosts, categories);
            }
            
            result.Success(data);
            
            // Lưu cache
            await cacheService.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

            logger.Information("END {MethodName} - Successfully retrieved post with slug: {PostSlug}", methodName,
                request.Slug);
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